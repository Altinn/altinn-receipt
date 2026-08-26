using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Altinn.Platform.Receipt.Configuration;
using Altinn.Platform.Receipt.Helpers;
using Altinn.Platform.Receipt.Model;
using Altinn.Platform.Receipt.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Altinn.Platform.Receipt.Services;

/// <summary>
/// Retrieves the list of Altinn organisations from the Altinn CDN.
/// </summary>
public class AltinnOrganisationsWrapper : IAltinnOrganisations
{
    private const string CacheKey = "altinn-orgs";

    private static readonly TimeSpan _cacheDuration = TimeSpan.FromHours(1);
    private static readonly TimeSpan _failedCacheDuration = TimeSpan.FromMinutes(1);

    private readonly HttpClient _client;
    private readonly IMemoryCache _cache;
    private readonly ILogger<AltinnOrganisationsWrapper> _logger;
    private readonly string _url;

    /// <summary>
    /// Initializes a new instance of the <see cref="AltinnOrganisationsWrapper"/> class
    /// </summary>
    public AltinnOrganisationsWrapper(
        HttpClient httpClient,
        IMemoryCache cache,
        ILogger<AltinnOrganisationsWrapper> logger,
        IOptions<GeneralSettings> generalSettings
    )
    {
        _client = httpClient;
        _cache = cache;
        _logger = logger;
        _url = generalSettings.Value.AltinnOrganisationsUrl;
    }

    /// <inheritdoc/>
    public async Task<AltinnOrgs> GetOrgs()
    {
        if (_cache.TryGetValue(CacheKey, out AltinnOrgs cached))
        {
            return cached;
        }

        AltinnOrgs orgs = await FetchOrgs();

        _cache.Set(CacheKey, orgs, orgs.Orgs.Count > 0 ? _cacheDuration : _failedCacheDuration);

        return orgs;
    }

    private async Task<AltinnOrgs> FetchOrgs()
    {
        try
        {
            AltinnOrgs orgs = await _client.GetFromJsonAsync<AltinnOrgs>(_url, JsonSerializerOptionsProvider.Options);

            if (orgs?.Orgs != null)
            {
                return orgs;
            }

            _logger.LogWarning("Received no Altinn organisations from {Url}.", _url);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Unable to retrieve Altinn organisations from {Url}.", _url);
        }

        return new AltinnOrgs { Orgs = new Dictionary<string, AltinnOrg>() };
    }
}
