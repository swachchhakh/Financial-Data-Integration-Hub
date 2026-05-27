using TransactionApi.Models;
using TransactionApi.Services;

namespace TransactionApi.Tests;

public class FraudCheckServiceTests
{
    private readonly FraudCheckService _sut = new();

    [Fact]
    public void Check_ValidTransaction_ReturnsApproved()
    {
        var transaction = new Transaction("txn-001", "acc-123", 250.00m, "Coffee Shop", "5812", DateTimeOffset.UtcNow);
        var result = _sut.Check(transaction);
        Assert.Equal("approved", result.Status);
        Assert.Null(result.Reason);
    }

    [Fact]
    public void Check_HighValueTransaction_ReturnsFlagged()
    {
        var transaction = new Transaction("txn-002", "acc-456", 15000.00m, "Unknown", "5812", DateTimeOffset.UtcNow);
        var result = _sut.Check(transaction);
        Assert.Equal("flagged", result.Status);
        Assert.Contains("High-value", result.Reason);
    }

    [Fact]
    public void Check_HighRiskMcc_ReturnsFlagged()
    {
        var transaction = new Transaction("txn-003", "acc-789", 50.00m, "Shady Merchant", "7995", DateTimeOffset.UtcNow);
        var result = _sut.Check(transaction);
        Assert.Equal("flagged", result.Status);
        Assert.Contains("7995", result.Reason);
    }

    [Fact]
    public void Check_ZeroAmount_ReturnsRejected()
    {
        var transaction = new Transaction("txn-004", "acc-123", 0m, "Coffee Shop", "5812", DateTimeOffset.UtcNow);
        var result = _sut.Check(transaction);
        Assert.Equal("rejected", result.Status);
    }

    [Fact]
    public void Check_NegativeAmount_ReturnsRejected()
    {
        var transaction = new Transaction("txn-005", "acc-123", -100m, "Coffee Shop", "5812", DateTimeOffset.UtcNow);
        var result = _sut.Check(transaction);
        Assert.Equal("rejected", result.Status);
    }

    [Theory]
    [InlineData("7995")]
    [InlineData("5933")]
    [InlineData("6051")]
    public void Check_AllHighRiskMccs_ReturnsFlagged(string mcc)
    {
        var transaction = new Transaction("txn-006", "acc-123", 100m, "Merchant", mcc, DateTimeOffset.UtcNow);
        var result = _sut.Check(transaction);
        Assert.Equal("flagged", result.Status);
    }
}
