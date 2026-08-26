using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Altinn.Platform.Receipt.Helpers;

/// <summary>
/// The texts used by the receipt page, and the rules for overriding them with app specific text resources.
/// </summary>
/// <remarks>
/// An app can override any of these texts by adding a text resource with the key
/// <c>receipt_platform.{key}</c>, for instance <c>receipt_platform.helper_text</c>.
/// </remarks>
public static class ReceiptTexts
{
    /// <summary>
    /// The prefix used by app text resources that override a receipt text.
    /// </summary>
    public const string TextResourcePrefix = "receipt_platform.";

    /// <summary>
    /// The languages the receipt page provides texts for. The first entry is the default language.
    /// </summary>
    public static IReadOnlyList<string> SupportedLanguages { get; } = new[] { "nb", "nn", "en" };

    private static readonly Dictionary<string, IReadOnlyDictionary<string, string>> _defaults = new()
    {
        ["nb"] = new ReadOnlyDictionary<string, string>(
            new Dictionary<string, string>
            {
                ["attachments"] = "Vedlegg",
                ["back_to_inbox"] = "Tilbake til innboks",
                ["date_archived"] = "Dato arkivert",
                ["date_sent"] = "Dato sendt",
                ["download"] = "last ned",
                ["error_no_access"] = "Du har ikke tilgang til denne kvitteringen.",
                ["error_not_found"] = "Vi finner ikke denne kvitteringen.",
                ["error_title"] = "Beklager, noe gikk galt",
                ["error_unknown"] = "Prøv igjen senere.",
                ["helper_text"] =
                    "Det er gjennomført en maskinell kontroll under utfylling, men vi tar forbehold om at det kan bli "
                    + "oppdaget feil under saksbehandlingen og at annen dokumentasjon kan være nødvendig. Vennligst "
                    + "oppgi referansenummer ved eventuelle henvendelser til etaten.",
                ["helper_text_a2lookup"] =
                    "Informasjonen som ble hentet ut fra det offentlige, er lagret og signert i Altinn. Klikk på "
                    + "lenke/vedlegg for å se informasjonen i et eget vindu.",
                ["is_sent"] = "er sendt inn",
                ["log_out"] = "Logg ut",
                ["receipt"] = "Kvittering",
                ["receiver"] = "Mottaker",
                ["reference_number"] = "Referansenummer",
                ["sender"] = "Avsender",
                ["sent_content"] = "Følgende er sendt inn:",
            }
        ),
        ["nn"] = new ReadOnlyDictionary<string, string>(
            new Dictionary<string, string>
            {
                ["attachments"] = "Vedlegg",
                ["back_to_inbox"] = "Tilbake til innboks",
                ["date_archived"] = "Dato arkivert",
                ["date_sent"] = "Dato sendt",
                ["download"] = "last ned",
                ["error_no_access"] = "Du har ikkje tilgang til denne kvitteringa.",
                ["error_not_found"] = "Vi finn ikkje denne kvitteringa.",
                ["error_title"] = "Beklagar, noko gjekk gale",
                ["error_unknown"] = "Prøv igjen seinare.",
                ["helper_text"] =
                    "Det er gjennomført ein maskinell kontroll under utfylling, men vi tek atterhald om at det kan bli "
                    + "oppdaga feil under sakshandsaminga og at annan dokumentasjon kan vere naudsynt. Ver venleg "
                    + "oppgi referansenummer ved eventuelle førespurnadar til etaten.",
                ["helper_text_a2lookup"] =
                    "Informasjonen som blei henta ut frå det offentlege, er lagra og signert i Altinn. Klikk på "
                    + "lenke/vedlegg for å sjå informasjonen i eit eige vindu.",
                ["is_sent"] = "er sendt inn",
                ["log_out"] = "Logg ut",
                ["receipt"] = "Kvittering",
                ["receiver"] = "Mottakar",
                ["reference_number"] = "Referansenummer",
                ["sender"] = "Avsendar",
                ["sent_content"] = "Følgjande er sendt inn:",
            }
        ),
        ["en"] = new ReadOnlyDictionary<string, string>(
            new Dictionary<string, string>
            {
                ["attachments"] = "Attachments",
                ["back_to_inbox"] = "Back to inbox",
                ["date_archived"] = "Date archived",
                ["date_sent"] = "Date sent",
                ["download"] = "download",
                ["error_no_access"] = "You do not have access to this receipt.",
                ["error_not_found"] = "We cannot find this receipt.",
                ["error_title"] = "Sorry, something went wrong",
                ["error_unknown"] = "Please try again later.",
                ["helper_text"] =
                    "A mechanical check has been completed while filling in, but we reserve the right to detect errors "
                    + "during the processing of the case and that other documentation may be necessary. Please provide "
                    + "the reference number in case of any inquiries to the agency.",
                ["helper_text_a2lookup"] =
                    "The information that was collected from the public sector is saved and signed in Altinn. Click "
                    + "the link/attachment to view the information in a separate window.",
                ["is_sent"] = "is submitted",
                ["log_out"] = "Log out",
                ["receipt"] = "Receipt",
                ["receiver"] = "Receiver",
                ["reference_number"] = "Reference number",
                ["sender"] = "Sender",
                ["sent_content"] = "The following is submitted:",
            }
        ),
    };

    /// <summary>
    /// Gets the default receipt texts for a language, falling back to Norwegian bokmål for unknown languages.
    /// </summary>
    /// <param name="language">The two letter language code.</param>
    /// <returns>The default texts.</returns>
    public static IReadOnlyDictionary<string, string> GetDefaults(string language)
    {
        if (!string.IsNullOrEmpty(language) && _defaults.TryGetValue(language, out var texts))
        {
            return texts;
        }

        return _defaults[SupportedLanguages[0]];
    }

    /// <summary>
    /// Orders the supported languages so that the preferred language is attempted first.
    /// </summary>
    /// <param name="preferredLanguage">The language preferred by the user.</param>
    /// <returns>The languages to attempt, in order.</returns>
    public static IReadOnlyList<string> GetLanguagePriority(string preferredLanguage)
    {
        List<string> languages = new();

        if (!string.IsNullOrEmpty(preferredLanguage))
        {
            languages.Add(preferredLanguage);
        }

        foreach (string language in SupportedLanguages)
        {
            if (!languages.Contains(language))
            {
                languages.Add(language);
            }
        }

        return languages;
    }
}
