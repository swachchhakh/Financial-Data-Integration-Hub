using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TransactionApi.Services;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddSingleton(sp =>
{
    var connStr = Environment.GetEnvironmentVariable("ServiceBusConnection");
    return string.IsNullOrEmpty(connStr)
        ? null!
        : new ServiceBusClient(connStr);
});

builder.Services.AddSingleton<FraudCheckService>();

builder.Build().Run();