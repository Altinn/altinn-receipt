using Altinn.Platform.Receipt.Helpers;
using Xunit;

namespace Altinn.Platform.Receipt.Tests.Helpers;

public class MarkdownHelperTests
{
    [Fact]
    public void ToHtml_PlainText_IsWrappedInParagraph()
    {
        Assert.Equal("<p>Kvittering</p>", MarkdownHelper.ToHtml("Kvittering"));
    }

    [Fact]
    public void ToHtml_Link_OpensInNewTab()
    {
        string html = MarkdownHelper.ToHtml("Se [vilkårene](https://www.altinn.no)");

        Assert.Equal(
            "<p>Se <a rel=\"noopener noreferrer\" target=\"_blank\" href=\"https://www.altinn.no\">vilkårene</a></p>",
            html
        );
    }

    [Fact]
    public void ToHtml_RawHtml_IsEscaped()
    {
        string html = MarkdownHelper.ToHtml("<script>alert('x')</script>");

        Assert.DoesNotContain("<script>", html);
        Assert.Contains("&lt;script&gt;", html);
    }

    [Fact]
    public void ToHtml_NoText_ReturnsEmptyString()
    {
        Assert.Equal(string.Empty, MarkdownHelper.ToHtml(null));
    }

    [Fact]
    public void ToInlineHtml_SingleParagraph_HasNoParagraphElement()
    {
        Assert.Equal("er sendt inn", MarkdownHelper.ToInlineHtml("er sendt inn"));
    }

    [Fact]
    public void ToInlineHtml_Emphasis_IsKept()
    {
        Assert.Equal("er <strong>sendt inn</strong>", MarkdownHelper.ToInlineHtml("er **sendt inn**"));
    }

    [Fact]
    public void ToInlineHtml_MultipleParagraphs_AreKept()
    {
        Assert.Equal("<p>en</p>\n<p>to</p>", MarkdownHelper.ToInlineHtml("en\n\nto"));
    }
}
