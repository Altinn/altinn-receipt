using System;
using Altinn.Platform.Profile.Models;
using Altinn.Platform.Receipt.Model;
using Altinn.Platform.Register.Models;
using Altinn.Platform.Storage.Interface.Models;

namespace Altinn.Platform.Receipt.ViewModels;

/// <summary>
/// The data the receipt page is built from.
/// </summary>
public class ReceiptPageContext
{
    /// <summary>
    /// The instance the receipt is shown for.
    /// </summary>
    public Instance Instance { get; init; }

    /// <summary>
    /// The guid of the instance.
    /// </summary>
    public Guid InstanceGuid { get; init; }

    /// <summary>
    /// The party that owns the instance.
    /// </summary>
    public Party Party { get; init; }

    /// <summary>
    /// The profile of the logged in user.
    /// </summary>
    public UserProfile User { get; init; }

    /// <summary>
    /// The application metadata of the app the instance belongs to.
    /// </summary>
    public Application Application { get; init; }

    /// <summary>
    /// The text resources of the app, in the language of the user.
    /// </summary>
    public TextResource TextResource { get; init; }

    /// <summary>
    /// The Altinn organisations.
    /// </summary>
    public AltinnOrgs Organisations { get; init; }

    /// <summary>
    /// The two letter language code the receipt is presented in.
    /// </summary>
    public string Language { get; init; }

    /// <summary>
    /// The host of the current request, including port.
    /// </summary>
    public string Host { get; init; }

    /// <summary>
    /// The return URL requested through the query string, if any.
    /// </summary>
    public string RequestedReturnUrl { get; init; }

    /// <summary>
    /// The semicolon separated attachment groups that should not be shown on the receipt.
    /// </summary>
    public string AttachmentGroupsToHide { get; init; }
}
