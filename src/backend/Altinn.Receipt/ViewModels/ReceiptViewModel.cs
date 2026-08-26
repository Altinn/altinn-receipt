using System.Collections.Generic;
using Microsoft.AspNetCore.Html;

namespace Altinn.Platform.Receipt.ViewModels;

/// <summary>
/// Everything the receipt page presents.
/// </summary>
public class ReceiptViewModel
{
    /// <summary>
    /// The two letter language code the page is presented in.
    /// </summary>
    public string Language { get; init; }

    /// <summary>
    /// The heading of the receipt, and the title of the page.
    /// </summary>
    public string Heading { get; init; }

    /// <summary>
    /// The name of the receiver of the submitted form.
    /// </summary>
    public string Receiver { get; init; }

    /// <summary>
    /// The name of the app, followed by a confirmation that it has been submitted.
    /// </summary>
    public HtmlString Title { get; init; }

    /// <summary>
    /// The explanatory text of the receipt.
    /// </summary>
    public HtmlString Body { get; init; }

    /// <summary>
    /// The key information about the submission.
    /// </summary>
    public IReadOnlyList<ReceiptMetaDataItem> MetaData { get; init; }

    /// <summary>
    /// The substatus set on the instance by the app, if any.
    /// </summary>
    public ReceiptSubstatus Substatus { get; init; }

    /// <summary>
    /// The heading shown above the submitted form, if any.
    /// </summary>
    public string SubmittedHeading { get; init; }

    /// <summary>
    /// The generated PDFs of the submitted form.
    /// </summary>
    public IReadOnlyList<ReceiptAttachment> Pdfs { get; init; }

    /// <summary>
    /// The attachments uploaded to the instance, grouped as the app has configured them.
    /// </summary>
    public IReadOnlyList<ReceiptAttachmentGroup> AttachmentGroups { get; init; }

    /// <summary>
    /// The text shown after an attachment name, telling the user that the link downloads the attachment.
    /// </summary>
    public string DownloadText { get; init; }

    /// <summary>
    /// The URL the user is sent to when leaving the receipt, if any.
    /// </summary>
    public string ReturnUrl { get; init; }

    /// <summary>
    /// The text of the link back to the inbox.
    /// </summary>
    public string ReturnText { get; init; }

    /// <summary>
    /// The name of the logged in user.
    /// </summary>
    public string UserName { get; init; }

    /// <summary>
    /// The name of the instance owner, when the user is not the instance owner.
    /// </summary>
    public string OnBehalfOfName { get; init; }

    /// <summary>
    /// The URL used to log the user out of Altinn, if any.
    /// </summary>
    public string LogoutUrl { get; init; }

    /// <summary>
    /// The text of the log out link.
    /// </summary>
    public string LogoutText { get; init; }
}

/// <summary>
/// A label and value pair in the key information about the submission.
/// </summary>
public class ReceiptMetaDataItem
{
    /// <summary>
    /// The label of the value.
    /// </summary>
    public string Label { get; init; }

    /// <summary>
    /// The value.
    /// </summary>
    public string Value { get; init; }
}

/// <summary>
/// The substatus set on the instance by the app.
/// </summary>
public class ReceiptSubstatus
{
    /// <summary>
    /// The label of the substatus.
    /// </summary>
    public string Label { get; init; }

    /// <summary>
    /// The description of the substatus.
    /// </summary>
    public string Description { get; init; }
}
