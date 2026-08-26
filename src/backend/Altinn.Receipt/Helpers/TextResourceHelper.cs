using System.Collections.Generic;
using Altinn.Platform.Receipt.Model;
using Altinn.Platform.Storage.Interface.Models;

namespace Altinn.Platform.Receipt.Helpers;

/// <summary>
/// Resolves values from the text resources of an app.
/// </summary>
public static class TextResourceHelper
{
    private const string AppNameKey = "appName";
    private const string LegacyAppNameKey = "ServiceName";
    private const string AppReceiverKey = "appReceiver";
    private const string DefaultLanguage = "nb";

    /// <summary>
    /// Gets the value of a text resource, or the key itself when the app has no text resource with that key.
    /// </summary>
    /// <param name="key">The text resource key.</param>
    /// <param name="textResource">The text resources of the app.</param>
    /// <returns>The text resource value, or <paramref name="key"/> when it is not found.</returns>
    public static string GetValue(string key, TextResource textResource)
    {
        if (string.IsNullOrEmpty(key) || textResource?.Resources == null)
        {
            return key;
        }

        foreach (TextResourceElement element in textResource.Resources)
        {
            if (element.Id == key)
            {
                return element.Value;
            }
        }

        return key;
    }

    /// <summary>
    /// Gets the name of the app, preferring the app text resources over the application metadata title.
    /// </summary>
    /// <param name="textResource">The text resources of the app.</param>
    /// <param name="application">The application metadata.</param>
    /// <param name="language">The two letter language code.</param>
    /// <returns>The app name, or an empty string when no name is available.</returns>
    public static string GetAppName(TextResource textResource, Application application, string language)
    {
        string appName = GetValue(AppNameKey, textResource);
        if (appName == AppNameKey)
        {
            appName = GetValue(LegacyAppNameKey, textResource);
        }

        if (appName != AppNameKey && appName != LegacyAppNameKey)
        {
            return appName;
        }

        return GetFromLanguageDictionary(application?.Title, language) ?? string.Empty;
    }

    /// <summary>
    /// Gets the receiver of the submitted form, preferring the app text resources over the organisation name.
    /// </summary>
    /// <param name="textResource">The text resources of the app.</param>
    /// <param name="organisations">The Altinn organisations.</param>
    /// <param name="org">The short name of the app owner.</param>
    /// <param name="language">The two letter language code.</param>
    /// <returns>The receiver name, or an empty string when no name is available.</returns>
    public static string GetAppReceiver(
        TextResource textResource,
        AltinnOrgs organisations,
        string org,
        string language
    )
    {
        string appReceiver = GetValue(AppReceiverKey, textResource);
        if (appReceiver != AppReceiverKey)
        {
            return appReceiver;
        }

        return GetOrgName(organisations, org, language) ?? string.Empty;
    }

    /// <summary>
    /// Gets the display name of an Altinn organisation.
    /// </summary>
    /// <param name="organisations">The Altinn organisations.</param>
    /// <param name="org">The short name of the organisation.</param>
    /// <param name="language">The two letter language code.</param>
    /// <returns>The organisation name, or null when the organisation is unknown.</returns>
    public static string GetOrgName(AltinnOrgs organisations, string org, string language)
    {
        if (string.IsNullOrEmpty(org) || organisations?.Orgs == null)
        {
            return null;
        }

        if (!organisations.Orgs.TryGetValue(org, out AltinnOrg organisation))
        {
            return null;
        }

        return GetFromLanguageDictionary(organisation.Name, language);
    }

    private static string GetFromLanguageDictionary(IReadOnlyDictionary<string, string> values, string language)
    {
        if (values == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(language) && values.TryGetValue(language, out string value))
        {
            return value;
        }

        return values.TryGetValue(DefaultLanguage, out string defaultValue) ? defaultValue : null;
    }
}
