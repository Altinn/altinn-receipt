namespace Altinn.Platform.Receipt.Helpers;

/// <summary>
/// Provides helper methods for language-related operations.
/// </summary>
public static class LanguageHelper
{
    /// <summary>
    /// Gets the language from the Altinn persistence cookie.
    /// </summary>
    /// <param name="cookieValue">The value of the Altinn persistence cookie containing language information.</param>
    /// <param name="defaultLang">The default language to return if the cookie is not found or doesn't contain language information.</param>
    /// <returns>The language code ('en', 'nb', 'nn') extracted from the Altinn persistence cookie, or the default language if not found.</returns>
    public static string GetLanguageFromAltinnPersistenceCookie(string cookieValue, string defaultLang = "nb")
    {
        if (string.IsNullOrEmpty(cookieValue))
        {
            return defaultLang;
        }

        if (cookieValue.Contains("UL=1033"))
        {
            return "en";
        }

        if (cookieValue.Contains("UL=1044"))
        {
            return "nb";
        }

        if (cookieValue.Contains("UL=2068"))
        {
            return "nn";
        }
        
        return defaultLang;
    }
}
