using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Infrastructure.Services.Communications;

var host = new HostBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        builder.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
        // etc...
    })
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        // Register your SMS service as ISmsService
        services.AddSingleton<ISmsService, AzureSmsService>();
    })
    .Build();

host.Run();
