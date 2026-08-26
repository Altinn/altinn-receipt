using System.Collections.Generic;
using System.Linq;
using Altinn.Platform.Receipt.Helpers;
using Altinn.Platform.Register.Models;
using Altinn.Platform.Storage.Interface.Models;
using Microsoft.AspNetCore.Html;

namespace Altinn.Platform.Receipt.ViewModels;

/// <summary>
/// Builds the view model of the receipt page.
/// </summary>
public static class ReceiptViewModelFactory
{
    private const string A2ServiceTypeDataValue = "A2ServiceType";
    private const string A2LookupServiceType = "Lookup";
    private const string DialogIdDataValue = "dialog.id";

    /// <summary>
    /// Builds the view model of the receipt page.
    /// </summary>
    /// <param name="context">The data the page is built from.</param>
    /// <returns>The view model.</returns>
    public static ReceiptViewModel Create(ReceiptPageContext context)
    {
        Instance instance = context.Instance;
        IReadOnlyDictionary<string, string> texts = ReceiptTextResolver.Resolve(
            context.Language,
            context.TextResource,
            instance
        );

        bool isA2Lookup = IsA2Lookup(instance);
        string appName = TextResourceHelper.GetAppName(context.TextResource, context.Application, context.Language);
        string receiver = TextResourceHelper.GetAppReceiver(
            context.TextResource,
            context.Organisations,
            instance?.Org,
            context.Language
        );

        return new ReceiptViewModel
        {
            Language = context.Language,
            Heading = texts["receipt"],
            Receiver = receiver,
            Title = BuildTitle(appName, isA2Lookup ? null : texts["is_sent"]),
            Body = new HtmlString(
                MarkdownHelper.ToHtml(isA2Lookup ? texts["helper_text_a2lookup"] : texts["helper_text"])
            ),
            MetaData = BuildMetaData(context, texts, receiver, isA2Lookup),
            Substatus = BuildSubstatus(instance, context.TextResource),
            SubmittedHeading = isA2Lookup ? null : texts["sent_content"],
            Pdfs = AttachmentHelper.GetPdfAttachments(instance, context.Host),
            AttachmentGroups = AttachmentHelper.GetAttachmentGroups(
                instance,
                context.Application,
                context.TextResource,
                AttachmentHelper.ParseGroupsToHide(context.AttachmentGroupsToHide),
                texts["attachments"],
                context.Host
            ),
            DownloadText = texts["download"],
            ReturnUrl = ReceiptUrlHelper.GetReturnUrl(
                context.Host,
                context.RequestedReturnUrl,
                GetPartyId(instance),
                GetDialogId(instance)
            ),
            ReturnText = texts["back_to_inbox"],
            UserName = context.User?.Party?.Name,
            OnBehalfOfName = GetOnBehalfOfName(context.User?.Party, context.Party),
            LogoutUrl = ReceiptUrlHelper.GetLogoutUrl(context.Host),
            LogoutText = texts["log_out"],
        };
    }

    private static HtmlString BuildTitle(string appName, string isSentText)
    {
        string title = HtmlEncoding.Encoder.Encode(appName ?? string.Empty);

        if (!string.IsNullOrEmpty(isSentText))
        {
            title = $"{title} {MarkdownHelper.ToInlineHtml(isSentText)}";
        }

        return new HtmlString(title.Trim());
    }

    private static IReadOnlyList<ReceiptMetaDataItem> BuildMetaData(
        ReceiptPageContext context,
        IReadOnlyDictionary<string, string> texts,
        string receiver,
        bool isA2Lookup
    )
    {
        List<ReceiptMetaDataItem> metaData = new();

        Add(metaData, isA2Lookup ? texts["date_archived"] : texts["date_sent"], GetDateSubmitted(context));

        if (!isA2Lookup)
        {
            Add(metaData, texts["sender"], GetSender(context.Party));
            Add(metaData, texts["receiver"], receiver);
        }

        Add(metaData, texts["reference_number"], ReceiptUrlHelper.GetArchiveReference(context.InstanceGuid));

        return metaData;
    }

    private static void Add(List<ReceiptMetaDataItem> metaData, string label, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            metaData.Add(new ReceiptMetaDataItem { Label = label, Value = value });
        }
    }

    private static string GetDateSubmitted(ReceiptPageContext context)
    {
        Instance instance = context.Instance;

        DataElement formData = GetFormDataElement(instance, context.Application);
        if (formData != null)
        {
            return ReceiptDateFormatter.FormatDateTime(instance.Process?.Ended ?? formData.LastChanged);
        }

        if (instance?.Status?.IsArchived == true)
        {
            return ReceiptDateFormatter.FormatDateTime(instance.Status.Archived);
        }

        return null;
    }

    private static DataElement GetFormDataElement(Instance instance, Application application)
    {
        if (instance?.Data == null || instance.Data.Count == 0)
        {
            return null;
        }

        DataType formDataType = application?.DataTypes?.FirstOrDefault(dataType => dataType.AppLogic != null);
        if (formDataType == null)
        {
            return null;
        }

        return instance.Data.FirstOrDefault(dataElement => dataElement.DataType == formDataType.Id);
    }

    private static string GetSender(Party party)
    {
        if (party == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(party.SSN))
        {
            return $"{party.SSN}-{party.Name}";
        }

        if (!string.IsNullOrEmpty(party.OrgNumber))
        {
            return $"{party.OrgNumber}-{party.Name}";
        }

        return party.Name;
    }

    private static ReceiptSubstatus BuildSubstatus(Instance instance, TextResource textResource)
    {
        Substatus substatus = instance?.Status?.Substatus;
        if (substatus == null)
        {
            return null;
        }

        return new ReceiptSubstatus
        {
            Label = TextResourceHelper.GetValue(substatus.Label, textResource),
            Description = TextResourceHelper.GetValue(substatus.Description, textResource),
        };
    }

    private static string GetOnBehalfOfName(Party userParty, Party instanceOwnerParty)
    {
        if (userParty == null || instanceOwnerParty == null || userParty.PartyId == instanceOwnerParty.PartyId)
        {
            return null;
        }

        return instanceOwnerParty.Name;
    }

    private static bool IsA2Lookup(Instance instance)
    {
        return GetDataValue(instance, A2ServiceTypeDataValue) == A2LookupServiceType;
    }

    private static string GetDialogId(Instance instance)
    {
        return GetDataValue(instance, DialogIdDataValue);
    }

    private static string GetDataValue(Instance instance, string key)
    {
        if (instance?.DataValues == null)
        {
            return null;
        }

        return instance.DataValues.TryGetValue(key, out string value) ? value : null;
    }

    private static int? GetPartyId(Instance instance)
    {
        string partyId = instance?.InstanceOwner?.PartyId;

        return int.TryParse(partyId, out int parsed) ? parsed : null;
    }
}
