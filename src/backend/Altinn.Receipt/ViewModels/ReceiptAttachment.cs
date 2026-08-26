using System.Collections.Generic;

namespace Altinn.Platform.Receipt.ViewModels;

/// <summary>
/// A data element that is presented as a downloadable attachment on the receipt.
/// </summary>
public class ReceiptAttachment
{
    /// <summary>
    /// The name shown for the attachment.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// The URL the attachment can be downloaded from.
    /// </summary>
    public string Url { get; init; }

    /// <summary>
    /// The id of the data type of the attachment.
    /// </summary>
    public string DataType { get; init; }
}

/// <summary>
/// A group of attachments shown as one section on the receipt.
/// </summary>
public class ReceiptAttachmentGroup
{
    /// <summary>
    /// The heading of the group.
    /// </summary>
    public string Title { get; init; }

    /// <summary>
    /// The attachments in the group.
    /// </summary>
    public IReadOnlyList<ReceiptAttachment> Attachments { get; init; }
}
