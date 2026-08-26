using System.Collections.Generic;
using Altinn.Platform.Storage.Interface.Models;

namespace Altinn.Platform.Receipt.Helpers;

/// <summary>
/// Resolves the texts used by the receipt page by overriding the default texts with app text resources.
/// </summary>
public static class ReceiptTextResolver
{
    private const string InstanceContextDataSource = "instanceContext";

    /// <summary>
    /// Resolves the receipt texts for a language, applying any app specific overrides.
    /// </summary>
    /// <param name="language">The two letter language code.</param>
    /// <param name="textResource">The text resources of the app.</param>
    /// <param name="instance">The instance the receipt is shown for.</param>
    /// <returns>The resolved texts, keyed without the <see cref="ReceiptTexts.TextResourcePrefix"/> prefix.</returns>
    public static IReadOnlyDictionary<string, string> Resolve(
        string language,
        TextResource textResource,
        Instance instance
    )
    {
        Dictionary<string, string> texts = new(ReceiptTexts.GetDefaults(language));

        if (textResource?.Resources == null)
        {
            return texts;
        }

        foreach (TextResourceElement element in textResource.Resources)
        {
            if (element.Id == null || !element.Id.StartsWith(ReceiptTexts.TextResourcePrefix))
            {
                continue;
            }

            string key = element.Id[ReceiptTexts.TextResourcePrefix.Length..];
            if (key.Length == 0)
            {
                continue;
            }

            texts[key] = ReplaceVariables(element.Value, element.Variables, instance);
        }

        return texts;
    }

    private static string ReplaceVariables(string value, List<TextResourceVariable> variables, Instance instance)
    {
        if (string.IsNullOrEmpty(value) || variables == null)
        {
            return value;
        }

        for (int i = 0; i < variables.Count; i++)
        {
            TextResourceVariable variable = variables[i];
            value = value.Replace($"{{{i}}}", GetVariableValue(variable, instance));
        }

        return value;
    }

    private static string GetVariableValue(TextResourceVariable variable, Instance instance)
    {
        if (variable.DataSource == InstanceContextDataSource && instance != null)
        {
            string value = variable.Key switch
            {
                "instanceId" => instance.Id,
                "appId" => instance.AppId,
                "instanceOwnerPartyId" => instance.InstanceOwner?.PartyId,
                _ => null,
            };

            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }
        }

        return variable.Key ?? string.Empty;
    }
}
