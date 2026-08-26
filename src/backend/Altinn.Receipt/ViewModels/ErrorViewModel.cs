namespace Altinn.Platform.Receipt.ViewModels;

/// <summary>
/// The message shown when the receipt cannot be presented.
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// The two letter language code the page is presented in.
    /// </summary>
    public string Language { get; init; }

    /// <summary>
    /// The heading of the page.
    /// </summary>
    public string Heading { get; init; }

    /// <summary>
    /// The message explaining why the receipt cannot be presented.
    /// </summary>
    public string Message { get; init; }
}
