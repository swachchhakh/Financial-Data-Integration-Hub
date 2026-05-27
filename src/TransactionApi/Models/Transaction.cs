namespace TransactionApi.Models;

public record Transaction(
    string Id,
    string AccountId,
    decimal Amount,
    string MerchantName,
    string MerchantCategoryCode,  // MCC — key for fraud checks
    DateTimeOffset Timestamp
);

public record TransactionResult(
    string TransactionId,
    string Status,       // "approved" | "flagged" | "rejected"
    string? Reason
);