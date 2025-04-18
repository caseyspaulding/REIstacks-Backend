using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;

using REIstacks.Infrastructure;
using REIStacks.Api.Middleware;

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
                       host == "localhost" ||
                       host == "http://localhost:3000";
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

    // Add this part to handle file uploads
    c.OperationFilter<FileUploadOperationFilter>();
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

// Configure middleware
app.UseCors("ReistacksFrontend");
app.UseHttpsRedirection();


app.UseSwagger();
app.UseSwaggerUI();
app.MapOpenApi();


app.UseJwtCookieAuth();     // First process JWT
app.UseAuthentication();    // Then standard auth
app.UseAuthorization();     // Then authorization

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