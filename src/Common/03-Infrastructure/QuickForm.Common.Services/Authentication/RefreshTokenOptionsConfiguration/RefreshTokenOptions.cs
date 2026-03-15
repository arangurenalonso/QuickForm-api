namespace QuickForm.Common.Infrastructure;

public sealed class RefreshTokenOptions
{
    public int ExpirationDays { get; set; } = 7;
    public int LengthInBytes { get; set; } = 64;
    public string SecretKey { get; set; } = string.Empty;
}
