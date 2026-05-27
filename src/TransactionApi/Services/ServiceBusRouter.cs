using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TransactionApi.Models;

namespace TransactionApi.Services;

public class ServiceBusRouter(ServiceBusClient client, ILogger<ServiceBusRouter> logger)
{
    public async Task RouteAsync(Transaction transaction, TransactionResult result)
    {
        var queueName = result.Status == "approved" ? "transactions-approved" : "transactions-review";

        try
        {
            var sender = client.CreateSender(queueName);
            var message = new ServiceBusMessage(JsonSerializer.Serialize(transaction))
            {
                MessageId = transaction.Id,
                Subject = result.Status
            };
            await sender.SendMessageAsync(message);
            logger.LogInformation("Routed transaction {Id} to queue: {Queue}", transaction.Id, queueName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to route transaction {Id} to Service Bus", transaction.Id);
        }
    }
}
