namespace Altinn.Platform.Receipt.Helpers;

/// <summary>
/// Masks Norwegian national identity numbers (fødselsnummer and D-nummer).
/// </summary>
public static class NationalIdentityNumberMasking
{
    /// <summary>
    /// Number of leading characters (the DDMMYY birth date) kept visible when masking; the rest is replaced with '*'.
    /// </summary>
    private const int VisibleDigits = 6;

    /// <summary>
    /// <p>Masks a national identity number string: keeps the first <see cref="VisibleDigits"/> characters
    /// (the DDMMYY birth date) visible and replaces the rest with '*', e.g. "12345678901" becomes "123456*****".</p>
    /// <p>This is a defensive helper for not leaking SSNs: it deliberately does NOT validate the input, so a
    /// partial or malformed value is still masked rather than returned in the clear. Validating first would
    /// risk leaking any value that fails the check. <c>null</c> and empty values are returned unchanged.</p>
    /// </summary>
    /// <param name="nationalIdentityNumber">The national identity number to mask.</param>
    /// <returns>The masked national identity number.</returns>
    public static string Mask(string nationalIdentityNumber)
    {
        if (string.IsNullOrEmpty(nationalIdentityNumber))
        {
            return nationalIdentityNumber;
        }

        if (nationalIdentityNumber.Length <= VisibleDigits)
        {
            // Too short to keep any part visible, so mask the whole thing.
            return new string('*', nationalIdentityNumber.Length);
        }

        string visiblePart = nationalIdentityNumber.Substring(0, VisibleDigits);
        string maskedPart = new string('*', nationalIdentityNumber.Length - VisibleDigits);
        return visiblePart + maskedPart;
    }
}
