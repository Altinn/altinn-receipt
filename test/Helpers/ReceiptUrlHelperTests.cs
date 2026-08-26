using System;
using Altinn.Platform.Receipt.Helpers;
using Xunit;

namespace Altinn.Platform.Receipt.Tests.Helpers;

public class ReceiptUrlHelperTests
{
    [Fact]
    public void GetArchiveReference_ReturnsLastGroupOfInstanceGuid()
    {
        Guid instanceGuid = Guid.Parse("1c3a4b9d-cbbe-4146-b370-4164e925812b");

        Assert.Equal("4164e925812b", ReceiptUrlHelper.GetArchiveReference(instanceGuid));
    }

    [Theory]
    [InlineData("local.altinn.cloud", "http://local.altinn.cloud/")]
    [InlineData("localhost:5060", "http://localhost:5060/")]
    public void GetReturnUrl_LocalHost_ReturnsLocalRoot(string host, string expected)
    {
        Assert.Equal(expected, ReceiptUrlHelper.GetReturnUrl(host, null, 50001, null));
    }

    [Fact]
    public void GetReturnUrl_ProductionHost_RedirectsThroughAccessManagement()
    {
        string returnUrl = ReceiptUrlHelper.GetReturnUrl("platform.altinn.no", null, 50001, null);

        Assert.Equal(
            "https://am.ui.altinn.no/accessmanagement/api/v1/reportee/changeandredirect"
                + "?partyId=50001&goTo=https%3A%2F%2Faf.altinn.no%2F",
            returnUrl
        );
    }

    [Fact]
    public void GetReturnUrl_TestHostWithDialogId_PointsAtTheDialog()
    {
        string returnUrl = ReceiptUrlHelper.GetReturnUrl("ttd.apps.tt02.altinn.no", null, null, "a-dialog-id");

        Assert.Equal("https://af.tt02.altinn.no/inbox/a-dialog-id", returnUrl);
    }

    [Fact]
    public void GetReturnUrl_UnknownHost_ReturnsNull()
    {
        Assert.Null(ReceiptUrlHelper.GetReturnUrl("example.com", null, 50001, null));
    }

    [Fact]
    public void GetReturnUrl_RequestedUrlWithinAltinn_IsUsed()
    {
        string returnUrl = ReceiptUrlHelper.GetReturnUrl(
            "platform.tt02.altinn.no",
            "https://tt02.altinn.no/ui/messagebox",
            50001,
            null
        );

        Assert.Equal("https://tt02.altinn.no/ui/messagebox", returnUrl);
    }

    [Fact]
    public void GetReturnUrl_RequestedUrlOutsideAltinn_IsIgnored()
    {
        string returnUrl = ReceiptUrlHelper.GetReturnUrl(
            "platform.tt02.altinn.no",
            "https://evil.example.com/phishing",
            null,
            null
        );

        Assert.Equal("https://af.tt02.altinn.no/", returnUrl);
    }

    [Theory]
    [InlineData("/receipt/50001", true)]
    [InlineData("//evil.example.com", false)]
    [InlineData("https://tt02.altinn.no/ui/messagebox", true)]
    [InlineData("https://platform.tt02.altinn.no/receipt", true)]
    [InlineData("https://altinn.no.evil.example.com/", false)]
    [InlineData("javascript:alert(1)", false)]
    [InlineData("", false)]
    public void IsAllowedReturnUrl_OnlyAllowsRelativeAndAltinnUrls(string returnUrl, bool expected)
    {
        Assert.Equal(expected, ReceiptUrlHelper.IsAllowedReturnUrl("platform.tt02.altinn.no", returnUrl));
    }

    [Theory]
    [InlineData("local.altinn.cloud", "http://local.altinn.cloud/")]
    [InlineData("platform.altinn.no", "https://altinn.no/ui/authentication/LogOut")]
    [InlineData("platform.tt02.altinn.no", "https://tt02.altinn.no/ui/authentication/LogOut")]
    [InlineData("ttd.apps.tt02.altinn.no", "https://tt02.altinn.no/ui/authentication/LogOut")]
    [InlineData("example.com", null)]
    public void GetLogoutUrl_ReturnsAltinnLogoutUrl(string host, string expected)
    {
        Assert.Equal(expected, ReceiptUrlHelper.GetLogoutUrl(host));
    }

    [Fact]
    public void MakeUrlRelativeIfSameDomain_SameHost_ReturnsRelativeUrl()
    {
        string url = ReceiptUrlHelper.MakeUrlRelativeIfSameDomain(
            "https://platform.at22.altinn.cloud/storage/api/v1/instances/1/2/data/3",
            "platform.at22.altinn.cloud:443"
        );

        Assert.Equal("/storage/api/v1/instances/1/2/data/3", url);
    }

    [Fact]
    public void MakeUrlRelativeIfSameDomain_OtherHost_ReturnsUrlUnchanged()
    {
        string url = ReceiptUrlHelper.MakeUrlRelativeIfSameDomain(
            "https://platform.at22.altinn.cloud/storage/api/v1/instances/1/2/data/3",
            "local.altinn.cloud"
        );

        Assert.Equal("https://platform.at22.altinn.cloud/storage/api/v1/instances/1/2/data/3", url);
    }
}
