using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TransactionApi.Services;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var serviceBusConn = Environment.GetEnvironmentVariable("ServiceBusConnection");
if (!string.IsNullOrEmpty(serviceBusConn))
{
    builder.Services.AddSingleton(new ServiceBusClient(serviceBusConn));
    builder.Services.AddSingleton<ServiceBusRouter>();
}

builder.Services.AddSingleton<FraudCheckService>();

builder.Build().Run();
