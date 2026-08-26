using System.Collections.Generic;
using System.Linq;
using Altinn.Platform.Receipt.Helpers;
using Altinn.Platform.Receipt.Tests.Testdata;
using Altinn.Platform.Receipt.ViewModels;
using Altinn.Platform.Storage.Interface.Models;
using Xunit;

namespace Altinn.Platform.Receipt.Tests.Helpers;

public class AttachmentHelperTests
{
    private const string Host = "platform.at22.altinn.cloud";

    [Fact]
    public void GetPdfAttachments_ReturnsTheGeneratedPdf()
    {
        IReadOnlyList<ReceiptAttachment> pdfs = AttachmentHelper.GetPdfAttachments(Instances.ArchivedInstance, Host);

        ReceiptAttachment pdf = Assert.Single(pdfs);
        Assert.Equal("skjema.pdf", pdf.Name);
        Assert.StartsWith("/storage/api/v1/instances/", pdf.Url);
    }

    [Fact]
    public void GetAttachmentGroups_UngroupedAttachmentsComeFirst()
    {
        IReadOnlyList<ReceiptAttachmentGroup> groups = GetGroups();

        Assert.Equal(2, groups.Count);
        Assert.Equal("Vedlegg", groups[0].Title);
        Assert.Equal(["vedlegg.pdf"], groups[0].Attachments.Select(attachment => attachment.Name));
    }

    [Fact]
    public void GetAttachmentGroups_GroupTitleComesFromTheTextResources()
    {
        IReadOnlyList<ReceiptAttachmentGroup> groups = GetGroups();

        Assert.Equal("Andre vedlegg", groups[1].Title);
        Assert.Equal(["gruppert.pdf"], groups[1].Attachments.Select(attachment => attachment.Name));
    }

    [Fact]
    public void GetAttachmentGroups_HiddenGroupsAreNotIncluded()
    {
        IReadOnlyList<ReceiptAttachmentGroup> groups = GetGroups();

        Assert.DoesNotContain(
            "skjult.pdf",
            groups.SelectMany(group => group.Attachments).Select(attachment => attachment.Name)
        );
    }

    [Fact]
    public void GetAttachmentGroups_FormDataAndGeneratedPdfAreNotAttachments()
    {
        IEnumerable<string> dataTypes = GetGroups()
            .SelectMany(group => group.Attachments)
            .Select(attachment => attachment.DataType);

        Assert.DoesNotContain("default", dataTypes);
        Assert.DoesNotContain(AttachmentHelper.PdfDataType, dataTypes);
    }

    [Fact]
    public void GetAttachmentGroups_AppOwnedDataIsNotAnAttachment()
    {
        Instance instance = Instances.ArchivedInstance;
        instance.Data.Add(
            new DataElement
            {
                DataType = "appowned",
                Filename = "appowned.pdf",
                SelfLinks = new ResourceLinks { Platform = "https://platform.at22.altinn.cloud/data" },
            }
        );

        IEnumerable<string> names = AttachmentHelper
            .GetAttachmentGroups(
                instance,
                Applications.Application1,
                TextResources.Norwegian,
                AttachmentHelper.ParseGroupsToHide(Applications.HiddenGrouping),
                "Vedlegg",
                Host
            )
            .SelectMany(group => group.Attachments)
            .Select(attachment => attachment.Name);

        Assert.DoesNotContain("appowned.pdf", names);
    }

    [Fact]
    public void ParseGroupsToHide_SplitsOnSemicolon()
    {
        IReadOnlyCollection<string> groups = AttachmentHelper.ParseGroupsToHide("group.one;group.two;");

        Assert.Equal(2, groups.Count);
        Assert.Contains("group.one", groups);
        Assert.Contains("group.two", groups);
    }

    [Fact]
    public void ParseGroupsToHide_NoConfiguredValue_ReturnsEmpty()
    {
        Assert.Empty(AttachmentHelper.ParseGroupsToHide(null));
    }

    private static IReadOnlyList<ReceiptAttachmentGroup> GetGroups()
    {
        return AttachmentHelper.GetAttachmentGroups(
            Instances.ArchivedInstance,
            Applications.Application1,
            TextResources.Norwegian,
            AttachmentHelper.ParseGroupsToHide(Applications.HiddenGrouping),
            "Vedlegg",
            Host
        );
    }
}
