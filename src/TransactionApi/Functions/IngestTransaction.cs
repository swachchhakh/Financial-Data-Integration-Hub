using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using TransactionApi.Models;
using TransactionApi.Services;

namespace TransactionApi.Functions;

public class IngestTransaction(
    ILogger<IngestTransaction> logger,
    FraudCheckService fraudCheck)
{
    [Function("IngestTransaction")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "transactions")] HttpRequestData req)
    {
        var body = await req.ReadAsStringAsync();
        var transaction = JsonSerializer.Deserialize<Transaction>(body!,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (transaction is null)
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Invalid transaction payload");
            return bad;
        }

        logger.LogInformation("Received transaction {Id} for account {AccountId}, amount {Amount}",
            transaction.Id, transaction.AccountId, transaction.Amount);

        var checkResult = fraudCheck.Check(transaction);

        logger.LogInformation("Transaction {Id} result: {Status}", transaction.Id, checkResult.Status);

        var response = req.CreateResponse(HttpStatusCode.Accepted);
        await response.WriteAsJsonAsync(checkResult);
        return response;
    }
}
