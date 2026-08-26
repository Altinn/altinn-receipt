using System.Collections.Generic;

namespace Altinn.Platform.Receipt.Model;

/// <summary>
/// The list of Altinn organisations published by the Altinn CDN.
/// </summary>
public class AltinnOrgs
{
    /// <summary>
    /// The organisations, keyed by their short name.
    /// </summary>
    public Dictionary<string, AltinnOrg> Orgs { get; set; }
}

/// <summary>
/// An organisation that owns apps in Altinn.
/// </summary>
public class AltinnOrg
{
    /// <summary>
    /// The name of the organisation, keyed by two letter language code.
    /// </summary>
    public Dictionary<string, string> Name { get; set; }
}
