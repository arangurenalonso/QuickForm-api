namespace QuickForm.Common.Infrastructure;
public sealed class InboxMessage
{
    public Guid Id { get; init; }

    public string Type { get; init; }
    public DateTime OccurredOnUtc { get; init; }
    public string Content { get; init; }
    public InboxStatus Status { get; set; }

    public DateTime? ProcessedOnUtc { get; init; }

    public string? Error { get; init; }
}
