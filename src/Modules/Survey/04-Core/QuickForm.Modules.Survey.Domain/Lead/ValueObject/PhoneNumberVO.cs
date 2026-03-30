using System.Globalization;
using PhoneNumbers;

namespace QuickForm.Modules.Survey.Domain;

public sealed record PhoneNumberVO
{
    private static readonly PhoneNumberUtil _phoneUtil = PhoneNumberUtil.GetInstance();

    public string E164 { get; }
    public string RegionCode { get; }              // PE, US, CA
    public int CountryCallingCode { get; }         // 51, 1, 34
    public string NationalNumber { get; }          // Digits without country code
    public string InternationalFormat { get; }     // +51 987 654 321
    public string NationalFormat { get; }          // 987 654 321
    public string? Extension { get; }

    private PhoneNumberVO(
        string e164,
        string regionCode,
        int countryCallingCode,
        string nationalNumber,
        string internationalFormat,
        string nationalFormat,
        string? extension)
    {
        E164 = e164;
        RegionCode = regionCode;
        CountryCallingCode = countryCallingCode;
        NationalNumber = nationalNumber;
        InternationalFormat = internationalFormat;
        NationalFormat = nationalFormat;
        Extension = extension;
    }

    /// <summary>
    /// Creates a phone number from a national/local number plus selected region.
    /// Example: regionCode = "PE", rawInput = "987654321"
    /// </summary>
    public static PhoneNumberVO Create(string regionCode, string rawInput)
    {
        if (string.IsNullOrWhiteSpace(regionCode))
        {
            throw new ArgumentException("Region code is required.", nameof(regionCode));

        }

        if (string.IsNullOrWhiteSpace(rawInput))
        {
            throw new ArgumentException("Phone number is required.", nameof(rawInput));
        }

        regionCode = NormalizeRegionCode(regionCode);

        ValidateSupportedRegion(regionCode);

        PhoneNumber parsed;
        try
        {
            parsed = _phoneUtil.Parse(rawInput, regionCode);
        }
        catch (NumberParseException ex)
        {
            throw new ArgumentException($"Invalid phone number format: {ex.Message}", nameof(rawInput));
        }

        ValidateParsedPhone(parsed, regionCode);

        return Build(parsed, regionCode);
    }

    /// <summary>
    /// Creates a phone number from a full international number.
    /// Example: rawInput = "+51987654321"
    /// </summary>
    public static PhoneNumberVO CreateFromInternational(string rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
        {
            throw new ArgumentException("Phone number is required.", nameof(rawInput));
        }

        PhoneNumber parsed;
        try
        {
            parsed = _phoneUtil.Parse(rawInput, null);
        }
        catch (NumberParseException ex)
        {
            throw new ArgumentException($"Invalid international phone number format: {ex.Message}", nameof(rawInput));
        }

        if (!_phoneUtil.IsPossibleNumber(parsed))
        {
            throw new ArgumentException("Phone number is not possible.", nameof(rawInput));
        }

        if (!_phoneUtil.IsValidNumber(parsed))
        {
            throw new ArgumentException("Phone number is not valid.", nameof(rawInput));

        }

        var detectedRegion = _phoneUtil.GetRegionCodeForNumber(parsed);
        if (string.IsNullOrWhiteSpace(detectedRegion))
        {
            throw new ArgumentException("Could not determine region from phone number.", nameof(rawInput));
        }

        return Build(parsed, detectedRegion);
    }

    /// <summary>
    /// Useful when your UI sends country dial code and local number separately,
    /// but ISO region is still preferred.
    /// </summary>
    public static PhoneNumberVO CreateFromDialCode(string dialCode, string nationalNumber)
    {
        if (string.IsNullOrWhiteSpace(dialCode))
           { throw new ArgumentException("Dial code is required.", nameof(dialCode)); }

        if (string.IsNullOrWhiteSpace(nationalNumber))
          { throw new ArgumentException("National number is required.", nameof(nationalNumber)); }

        var combined = dialCode.StartsWith('+')
            ? $"{dialCode}{nationalNumber}"
            : $"+{dialCode}{nationalNumber}";

        return CreateFromInternational(combined);
    }

    public override string ToString() => E164;

    private static void ValidateSupportedRegion(string regionCode)
    {
        var supportedRegions = _phoneUtil.GetSupportedRegions();
        if (!supportedRegions.Contains(regionCode))
        {
            throw new ArgumentException($"Region code '{regionCode}' is not supported.", nameof(regionCode));
        }
    }

    private static void ValidateParsedPhone(PhoneNumber parsed, string expectedRegionCode)
    {
        if (!_phoneUtil.IsPossibleNumber(parsed))
           { throw new ArgumentException("Phone number is not possible for the selected country."); }

        if (!_phoneUtil.IsValidNumber(parsed))
           { throw new ArgumentException("Phone number is not valid for the selected country."); }

        var detectedRegion = _phoneUtil.GetRegionCodeForNumber(parsed);

        // Extra strict validation:
        // if the user selected PE, the parsed number must actually belong to PE.
        if (!string.Equals(detectedRegion, expectedRegionCode, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                $"The phone number does not belong to the selected country '{expectedRegionCode}'.");
        }
    }

    private static PhoneNumberVO Build(PhoneNumber parsed, string regionCode)
    {
        var e164 = _phoneUtil.Format(parsed, PhoneNumberFormat.E164);
        var international = _phoneUtil.Format(parsed, PhoneNumberFormat.INTERNATIONAL);
        var national = _phoneUtil.Format(parsed, PhoneNumberFormat.NATIONAL);

        return new PhoneNumberVO(
            e164: e164,
            regionCode: regionCode,
            countryCallingCode: parsed.CountryCode,
            nationalNumber: parsed.NationalNumber.ToString(CultureInfo.InvariantCulture),
            internationalFormat: international,
            nationalFormat: national,
            extension: string.IsNullOrWhiteSpace(parsed.Extension) ? null : parsed.Extension
        );
    }

    private static string NormalizeRegionCode(string regionCode)
        => regionCode.Trim().ToUpperInvariant();
}
