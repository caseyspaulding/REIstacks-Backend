using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;
using REIstacks.Infrastructure;
using REIStacks.Api.Middleware;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
// Add Infrastructure layer (repositories, services, etc.)
builder.Services.AddInfrastructure(builder.Configuration);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReistacksFrontend", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
        {
            if (Uri.TryCreate(origin, UriKind.Absolute, out var uri))
            {
                var host = uri.Host;
                return host == "reistacks.com" ||
                       host == "www.reistacks.com" ||
                       host.EndsWith(".reistacks.com") ||
                       host == "localhost";

            }
            return false;
        })
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .WithExposedHeaders("Access-Control-Allow-Origin");
    });
});

// Add controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.MaxDepth = 64;
    });

// Add API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "REIstacks API", Version = "v1" });
    c.OperationFilter<FileUploadOperationFilter>();
    c.AddSecurityDefinition("cookieAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Cookie,
        Name = "access_token",
        Scheme = "cookieAuth",
        Description = "Put your JWT access_token here"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "cookieAuth"
            }
        }
      ] = Array.Empty<string>()
    });
    // Optional: Add XML comments if you have them
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});
builder.Services.AddOpenApi();



// Add authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "access_token";
        options.Cookie.HttpOnly = true;

        if (builder.Environment.IsDevelopment())
        {
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.None;
        }
        else
        {
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.Domain = ".reistacks.com";
            options.Cookie.SameSite = SameSiteMode.None;
        }

        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            }
        };
    });




var app = builder.Build();
app.UseRouting();
// Configure middleware
app.UseHttpsRedirection();
app.UseCors("ReistacksFrontend");


app.UseJwtCookieAuth();     // First process JWT

app.UseAuthentication();    // Then standard auth

app.UseAuthorization();     // Then authorization

app.UseSwagger();
app.UseSwaggerUI();
app.MapOpenApi();
app.MapControllers();
app.MapGet("/health", () => "Healthy");

// Database migrations
try
{
    using (var scope = app.Services.CreateScope())
    {
        // Your migration code would go here if needed
        // Or it's handled by your UnitOfWork in the Infrastructure layer
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error during startup: {ex.Message}");
}

app.Run();