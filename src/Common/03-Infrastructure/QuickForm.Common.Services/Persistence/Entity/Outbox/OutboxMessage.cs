namespace QuickForm.Common.Infrastructure;
public sealed class OutboxMessage
{
    public Guid Id { get; init; }

    public string Type { get; init; }
    public DateTime OccurredOnUtc { get; init; }

    public string Content { get; init; }
    public OutboxStatus Status { get; set; }
    public DateTime? ProcessedOnUtc { get; init; }
    public string? Error { get; init; }
    public string ClassOrigin { get; set; }
    public Guid TransactionId { get; set; }
}
