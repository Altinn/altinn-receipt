using System.Threading.Tasks;
using Altinn.Platform.Receipt.Model;

namespace Altinn.Platform.Receipt.Services.Interfaces;

/// <summary>
/// Interface for the list of Altinn organisations.
/// </summary>
public interface IAltinnOrganisations
{
    /// <summary>
    /// Gets the Altinn organisations.
    /// </summary>
    /// <returns>The organisations, without any organisations if they could not be retrieved.</returns>
    public Task<AltinnOrgs> GetOrgs();
}
