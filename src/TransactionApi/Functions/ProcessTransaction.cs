using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TransactionApi.Models;

namespace TransactionApi.Functions;

public class ProcessTransaction(ILogger<ProcessTransaction> logger)
{
    [Function("ProcessApproved")]
    public void RunApproved(
        [ServiceBusTrigger("transactions-approved", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message)
    {
        var transaction = JsonSerializer.Deserialize<Transaction>(message.Body.ToString(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        logger.LogInformation(
            "Processing approved transaction {Id} for account {AccountId}, amount {Amount}, merchant {Merchant}",
            transaction?.Id, transaction?.AccountId, transaction?.Amount, transaction?.MerchantName);

        // Simulate downstream CRM/ledger call
        logger.LogInformation("Transaction {Id} posted to ledger successfully", transaction?.Id);
    }

    [Function("ProcessReview")]
    public void RunReview(
        [ServiceBusTrigger("transactions-review", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message)
    {
        var transaction = JsonSerializer.Deserialize<Transaction>(message.Body.ToString(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        logger.LogInformation(
            "Flagged transaction {Id} for account {AccountId}, amount {Amount} queued for manual review",
            transaction?.Id, transaction?.AccountId, transaction?.Amount);

        // Simulate alerting a compliance team
        logger.LogWarning("COMPLIANCE ALERT: Transaction {Id} requires manual review", transaction?.Id);
    }
}
