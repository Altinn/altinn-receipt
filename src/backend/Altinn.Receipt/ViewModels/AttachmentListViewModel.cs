using System.Collections.Generic;

namespace Altinn.Platform.Receipt.ViewModels;

/// <summary>
/// A list of attachments shown on the receipt.
/// </summary>
public class AttachmentListViewModel
{
    /// <summary>
    /// The id of the list element, if it needs one.
    /// </summary>
    public string Id { get; init; }

    /// <summary>
    /// The attachments in the list.
    /// </summary>
    public IReadOnlyList<ReceiptAttachment> Attachments { get; init; }

    /// <summary>
    /// The text shown after an attachment name, telling the user that the link downloads the attachment.
    /// </summary>
    public string DownloadText { get; init; }
}
