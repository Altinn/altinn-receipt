using System.Collections.Generic;
using System.Linq;
using Altinn.Platform.Receipt.ViewModels;
using Altinn.Platform.Storage.Interface.Models;

namespace Altinn.Platform.Receipt.Helpers;

/// <summary>
/// Turns the data elements of an instance into the attachments shown on the receipt.
/// </summary>
public static class AttachmentHelper
{
    /// <summary>
    /// The id of the data type holding the generated PDF of the submitted form.
    /// </summary>
    public const string PdfDataType = "ref-data-as-pdf";

    private const string AppOwnedContributor = "app:owned";

    /// <summary>
    /// Gets the generated PDFs of the submitted form.
    /// </summary>
    /// <param name="instance">The instance the receipt is shown for.</param>
    /// <param name="host">The host of the current request, including port.</param>
    /// <returns>The PDF attachments.</returns>
    public static IReadOnlyList<ReceiptAttachment> GetPdfAttachments(Instance instance, string host)
    {
        if (instance?.Data == null)
        {
            return [];
        }

        return instance
            .Data.Where(dataElement => dataElement.DataType == PdfDataType)
            .Select(dataElement => CreateAttachment(dataElement, host))
            .ToList();
    }

    /// <summary>
    /// Gets the attachments uploaded to the instance, grouped the way the app has configured its data types.
    /// </summary>
    /// <remarks>
    /// Data elements holding form data, data elements owned by the app and the generated PDF are not attachments.
    /// The remaining data elements are grouped by the grouping of their data type. Attachments without a grouping
    /// are placed first, in a group named by <paramref name="defaultTitle"/>.
    /// </remarks>
    /// <param name="instance">The instance the receipt is shown for.</param>
    /// <param name="application">The application metadata of the app.</param>
    /// <param name="textResource">The text resources of the app, used to name the groups.</param>
    /// <param name="groupsToHide">The groupings that should not be shown on the receipt.</param>
    /// <param name="defaultTitle">The heading used for attachments without a grouping.</param>
    /// <param name="host">The host of the current request, including port.</param>
    /// <returns>The attachment groups.</returns>
    public static IReadOnlyList<ReceiptAttachmentGroup> GetAttachmentGroups(
        Instance instance,
        Application application,
        TextResource textResource,
        IReadOnlyCollection<string> groupsToHide,
        string defaultTitle,
        string host
    )
    {
        if (instance?.Data == null || application == null)
        {
            return [];
        }

        HashSet<string> excludedDataTypes = GetExcludedDataTypes(application);
        Dictionary<string, string> groupingByDataType = GetGroupingByDataType(application);

        List<ReceiptAttachment> ungrouped = new();
        List<ReceiptAttachmentGroup> groups = new();
        Dictionary<string, List<ReceiptAttachment>> attachmentsByGrouping = new();

        foreach (DataElement dataElement in instance.Data)
        {
            if (dataElement.DataType != null && excludedDataTypes.Contains(dataElement.DataType))
            {
                continue;
            }

            groupingByDataType.TryGetValue(dataElement.DataType ?? string.Empty, out string grouping);

            if (grouping == null)
            {
                ungrouped.Add(CreateAttachment(dataElement, host));
                continue;
            }

            if (groupsToHide != null && groupsToHide.Contains(grouping))
            {
                continue;
            }

            if (!attachmentsByGrouping.TryGetValue(grouping, out List<ReceiptAttachment> attachments))
            {
                attachments = new List<ReceiptAttachment>();
                attachmentsByGrouping[grouping] = attachments;
                groups.Add(
                    new ReceiptAttachmentGroup
                    {
                        Title = TextResourceHelper.GetValue(grouping, textResource),
                        Attachments = attachments,
                    }
                );
            }

            attachments.Add(CreateAttachment(dataElement, host));
        }

        if (ungrouped.Count > 0)
        {
            groups.Insert(0, new ReceiptAttachmentGroup { Title = defaultTitle, Attachments = ungrouped });
        }

        return groups;
    }

    /// <summary>
    /// Splits the configured semicolon separated list of attachment groups to hide.
    /// </summary>
    /// <param name="attachmentGroupsToHide">The configured value.</param>
    /// <returns>The groupings that should not be shown on the receipt.</returns>
    public static IReadOnlyCollection<string> ParseGroupsToHide(string attachmentGroupsToHide)
    {
        if (string.IsNullOrEmpty(attachmentGroupsToHide))
        {
            return [];
        }

        return attachmentGroupsToHide
            .Split(';', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries)
            .ToHashSet();
    }

    private static HashSet<string> GetExcludedDataTypes(Application application)
    {
        HashSet<string> excluded = new() { PdfDataType };

        foreach (DataType dataType in application.DataTypes ?? [])
        {
            bool isFormData = dataType.AppLogic != null;
#pragma warning disable CS0618 // Apps created before the spelling was fixed still use the misspelled property.
            bool isAppOwned =
                dataType.AllowedContributers?.Contains(AppOwnedContributor) == true
                || dataType.AllowedContributors?.Contains(AppOwnedContributor) == true;
#pragma warning restore CS0618

            if (dataType.Id != null && (isFormData || isAppOwned))
            {
                excluded.Add(dataType.Id);
            }
        }

        return excluded;
    }

    private static Dictionary<string, string> GetGroupingByDataType(Application application)
    {
        Dictionary<string, string> groupings = new();

        foreach (DataType dataType in application.DataTypes ?? [])
        {
            if (dataType.Id != null && !string.IsNullOrEmpty(dataType.Grouping))
            {
                groupings[dataType.Id] = dataType.Grouping;
            }
        }

        return groupings;
    }

    private static ReceiptAttachment CreateAttachment(DataElement dataElement, string host)
    {
        return new ReceiptAttachment
        {
            Name = string.IsNullOrEmpty(dataElement.Filename) ? dataElement.DataType : dataElement.Filename,
            Url = ReceiptUrlHelper.MakeUrlRelativeIfSameDomain(dataElement.SelfLinks?.Platform, host),
            DataType = dataElement.DataType,
        };
    }
}
