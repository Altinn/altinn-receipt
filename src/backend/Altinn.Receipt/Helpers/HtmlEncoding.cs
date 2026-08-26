using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace Altinn.Platform.Receipt.Helpers;

/// <summary>
/// The HTML encoder used by the receipt views.
/// </summary>
/// <remarks>
/// The default encoder escapes every character outside basic latin, which turns the Norwegian letters of the
/// receipt into numeric character references. Latin-1 is left as is instead, since the pages are UTF-8 encoded.
/// </remarks>
public static class HtmlEncoding
{
    /// <summary>
    /// The encoder.
    /// </summary>
    public static HtmlEncoder Encoder { get; } =
        HtmlEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Latin1Supplement);
}
