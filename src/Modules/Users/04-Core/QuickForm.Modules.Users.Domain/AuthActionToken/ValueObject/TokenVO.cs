using QuickForm.Common.Domain;
namespace QuickForm.Modules.Users.Domain;

public record TokenVO
{
    public string Value { get; }

    private TokenVO(string value)
    {
        Value = value;
    }

    public static ResultT<TokenVO> Create(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return ResultError.EmptyValue("Token", "Token cannot be null or empty.");
        }

        // Validación de longitud del token
        if (token.Length < 32)
        {
            return ResultError.InvalidInput("Token", "Token must be at least 32 characters long.");
        }

        // Validación del contenido alfanumérico
        var textValidation = new TextValidationBuilder()
                                    .AddAlphanumericCharacters()
                                    .AddUnderscore()
                                    .AddHyphen()
                                    .Build().ValidateInvalidCharacter("Token", token);

        if (textValidation.IsFailure)
        {
            return textValidation.Errors;
        }

        return new TokenVO(token);
    }

    public static TokenVO GenerateNewToken(int length = 32)
    {
        var token = AuthTokenGenerator.GenerateAlphanumericToken(length);
        return new TokenVO(token);
    }

    public static implicit operator string(TokenVO token) => token.Value;
}
