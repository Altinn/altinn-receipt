using System;
using System.Text.RegularExpressions;

namespace Altinn.Platform.Receipt.Helpers;

/// <summary>
/// Builds the URLs used by the receipt page.
/// </summary>
public static partial class ReceiptUrlHelper
{
    private const string ProductionAltinnHost = "altinn.no";

    /// <summary>
    /// Gets the archive reference shown on the receipt, which is the last group of the instance guid.
    /// </summary>
    /// <param name="instanceGuid">The instance guid.</param>
    /// <returns>The archive reference.</returns>
    public static string GetArchiveReference(Guid instanceGuid)
    {
        string[] parts = instanceGuid.ToString().Split('-');
        return parts.Length == 5 ? parts[4] : string.Empty;
    }

    /// <summary>
    /// Gets the URL the user is sent to when leaving the receipt.
    /// </summary>
    /// <param name="host">The host of the current request, including port.</param>
    /// <param name="requestedReturnUrl">The return URL requested through the query string, if any.</param>
    /// <param name="partyId">The party id of the instance owner.</param>
    /// <param name="dialogId">The id of the dialogporten dialog for the instance, if any.</param>
    /// <returns>The return URL, or null when no return URL can be determined.</returns>
    public static string GetReturnUrl(string host, string requestedReturnUrl, int? partyId, string dialogId)
    {
        if (IsAllowedReturnUrl(host, requestedReturnUrl))
        {
            return requestedReturnUrl;
        }

        if (IsLocalHost(host))
        {
            return $"http://{host}/";
        }

        string altinnHost = GetAltinnHost(host);
        if (altinnHost == null)
        {
            return null;
        }

        string arbeidsflateUrl =
            altinnHost == ProductionAltinnHost ? "https://af.altinn.no/" : $"https://af.{altinnHost}/";

        string targetUrl = string.IsNullOrEmpty(dialogId)
            ? arbeidsflateUrl
            : $"{arbeidsflateUrl.TrimEnd('/')}/inbox/{dialogId}";

        if (partyId == null)
        {
            return targetUrl;
        }

        // Switch to the reportee of the instance through access management before redirecting to Arbeidsflate.
        return $"https://am.ui.{altinnHost}/accessmanagement/api/v1/reportee/changeandredirect"
            + $"?partyId={partyId}&goTo={Uri.EscapeDataString(targetUrl)}";
    }

    /// <summary>
    /// Gets the URL used to log the user out of Altinn.
    /// </summary>
    /// <param name="host">The host of the current request, including port.</param>
    /// <returns>The log out URL, or null when the host is not an Altinn host.</returns>
    public static string GetLogoutUrl(string host)
    {
        if (IsLocalHost(host))
        {
            return $"http://{host}/";
        }

        string altinnHost = GetAltinnHost(host);
        return altinnHost == null ? null : $"https://{altinnHost}/ui/authentication/LogOut";
    }

    /// <summary>
    /// Makes an absolute URL relative when it points to the host of the current request.
    /// </summary>
    /// <remarks>
    /// Storage always returns https links for data elements. On hosts served over plain http, such as the local
    /// development environment, following those links fails, so links to the current host are made relative.
    /// </remarks>
    /// <param name="url">The URL to rewrite.</param>
    /// <param name="host">The host of the current request, including port.</param>
    /// <returns>A relative URL when the URL points to the current host, otherwise the URL unchanged.</returns>
    public static string MakeUrlRelativeIfSameDomain(string url, string host)
    {
        if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(host))
        {
            return url;
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri parsed))
        {
            return url;
        }

        string hostWithoutPort = host.Split(':')[0];
        if (!string.Equals(parsed.Host, hostWithoutPort, StringComparison.OrdinalIgnoreCase))
        {
            return url;
        }

        return parsed.PathAndQuery + parsed.Fragment;
    }

    /// <summary>
    /// Determines whether a return URL requested through the query string can be used.
    /// </summary>
    /// <remarks>
    /// Only relative URLs and URLs within Altinn are allowed, to avoid turning the receipt into an open redirect.
    /// </remarks>
    /// <param name="host">The host of the current request, including port.</param>
    /// <param name="returnUrl">The return URL requested through the query string.</param>
    /// <returns>True when the return URL can be used.</returns>
    public static bool IsAllowedReturnUrl(string host, string returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl))
        {
            return false;
        }

        if (returnUrl.StartsWith('/'))
        {
            // Protocol relative URLs, such as //example.com, point to another host.
            return !returnUrl.StartsWith("//");
        }

        if (!Uri.TryCreate(returnUrl, UriKind.Absolute, out Uri parsed))
        {
            return false;
        }

        if (parsed.Scheme != Uri.UriSchemeHttps && parsed.Scheme != Uri.UriSchemeHttp)
        {
            return false;
        }

        if (
            !string.IsNullOrEmpty(host)
            && string.Equals(parsed.Host, host.Split(':')[0], StringComparison.OrdinalIgnoreCase)
        )
        {
            return true;
        }

        return AltinnHostRegex().IsMatch(parsed.Host);
    }

    private static bool IsLocalHost(string host)
    {
        return host != null && (LocalAltinnHostRegex().IsMatch(host) || LocalhostRegex().IsMatch(host));
    }

    private static string GetAltinnHost(string host)
    {
        if (host == null)
        {
            return null;
        }

        Match match = PlatformHostRegex().Match(host);
        return match.Success ? match.Groups[1].Value : null;
    }

    [GeneratedRegex(@"^(?:[a-zA-Z0-9_]+\.apps|platform)\.(([a-zA-Z0-9_]+\.)?altinn\.(no|cloud))$")]
    private static partial Regex PlatformHostRegex();

    [GeneratedRegex(@"^local\.altinn\.cloud(:\d+)?$")]
    private static partial Regex LocalAltinnHostRegex();

    [GeneratedRegex(@"^localhost(:\d+)?$")]
    private static partial Regex LocalhostRegex();

    [GeneratedRegex(@"^([a-zA-Z0-9_-]+\.)*altinn\.(no|cloud)$")]
    private static partial Regex AltinnHostRegex();
}
