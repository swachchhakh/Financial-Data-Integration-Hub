using TransactionApi.Models;

namespace TransactionApi.Services;

public class FraudCheckService
{
    // High-risk merchant category codes (simplified)
    private static readonly HashSet<string> HighRiskMccs = ["7995", "5933", "6051"];
    private const decimal HighValueThreshold = 10_000m;

    public TransactionResult Check(Transaction t)
    {
        if (t.Amount <= 0)
            return new(t.Id, "rejected", "Amount must be positive");

        if (t.Amount > HighValueThreshold)
            return new(t.Id, "flagged", $"High-value transaction: ${t.Amount:N2}");

        if (HighRiskMccs.Contains(t.MerchantCategoryCode))
            return new(t.Id, "flagged", $"High-risk merchant category: {t.MerchantCategoryCode}");

        return new(t.Id, "approved", null);
    }
}