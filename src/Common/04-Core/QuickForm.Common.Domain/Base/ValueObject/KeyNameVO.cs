namespace QuickForm.Common.Domain;
public sealed record KeyNameVO
{
    public string Value { get; private set; }

    private KeyNameVO(string value)
    {
        Value = value;
    }

    private KeyNameVO() { }

    public static ResultT<KeyNameVO> Create(string? keyName)
    {
        if (string.IsNullOrWhiteSpace(keyName))
        {
            return ResultError.EmptyValue("KeyName", "KeyName cannot be null or empty.");
        }
        var normalizedKeyName = keyName.Trim(); 
        return new KeyNameVO(normalizedKeyName);
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(KeyNameVO key) => key.Value;

}
