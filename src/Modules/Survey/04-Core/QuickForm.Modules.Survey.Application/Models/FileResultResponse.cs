namespace QuickForm.Common.Application;

public sealed record FileResultResponse(
    byte[] Content,
    string ContentType,
    string FileName
);
