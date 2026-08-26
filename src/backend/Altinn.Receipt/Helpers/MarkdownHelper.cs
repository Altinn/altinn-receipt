using System.Text.RegularExpressions;
using Markdig;

namespace Altinn.Platform.Receipt.Helpers;

/// <summary>
/// Renders the markdown that apps are allowed to use in the receipt texts.
/// </summary>
public static partial class MarkdownHelper
{
    private static readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder().DisableHtml().Build();

    /// <summary>
    /// Renders markdown as HTML.
    /// </summary>
    /// <remarks>
    /// Raw HTML in the markdown is escaped rather than rendered, so every anchor in the result is written by the
    /// markdown renderer. Those anchors are opened in a new tab, matching how the receipt has always rendered links.
    /// </remarks>
    /// <param name="markdown">The markdown to render.</param>
    /// <returns>The rendered HTML.</returns>
    public static string ToHtml(string markdown)
    {
        if (string.IsNullOrEmpty(markdown))
        {
            return string.Empty;
        }

        string html = Markdown.ToHtml(markdown, _pipeline).Trim();

        return AnchorRegex().Replace(html, "<a rel=\"noopener noreferrer\" target=\"_blank\" ");
    }

    /// <summary>
    /// Renders markdown as HTML without the paragraph element wrapping a single block of text.
    /// </summary>
    /// <param name="markdown">The markdown to render.</param>
    /// <returns>The rendered HTML, suitable for use inside an existing block element.</returns>
    public static string ToInlineHtml(string markdown)
    {
        string html = ToHtml(markdown);

        Match match = SingleParagraphRegex().Match(html);

        return match.Success ? match.Groups[1].Value : html;
    }

    [GeneratedRegex("<a ")]
    private static partial Regex AnchorRegex();

    [GeneratedRegex(@"\A<p>((?:(?!</p>).)*)</p>\z", RegexOptions.Singleline)]
    private static partial Regex SingleParagraphRegex();
}
