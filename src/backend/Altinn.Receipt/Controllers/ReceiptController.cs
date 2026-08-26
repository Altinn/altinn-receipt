using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Altinn.Platform.Profile.Models;
using Altinn.Platform.Receipt.Configuration;
using Altinn.Platform.Receipt.Helpers;
using Altinn.Platform.Receipt.Services.Interfaces;
using Altinn.Platform.Receipt.ViewModels;
using Altinn.Platform.Register.Models;
using Altinn.Platform.Storage.Interface.Models;
using AltinnCore.Authentication.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Altinn.Platform.Receipt.Controllers;

/// <summary>
/// Presents the receipt of a submitted form.
/// </summary>
[Authorize]
public class ReceiptController : Controller
{
    private const string LanguageCookieName = "altinnPersistentContext";

    private readonly IRegister _register;
    private readonly IStorage _storage;
    private readonly IProfile _profile;
    private readonly IAltinnOrganisations _organisations;
    private readonly ILogger<ReceiptController> _logger;
    private readonly IOptions<GeneralSettings> _generalSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReceiptController"/> class
    /// </summary>
    /// <param name="register">the register service</param>
    /// <param name="storage">the storage service</param>
    /// <param name="profile">the profile service</param>
    /// <param name="organisations">the Altinn organisations</param>
    /// <param name="logger">the logger</param>
    /// <param name="generalSettings">The application general settings</param>
    public ReceiptController(
        IRegister register,
        IStorage storage,
        IProfile profile,
        IAltinnOrganisations organisations,
        ILogger<ReceiptController> logger,
        IOptions<GeneralSettings> generalSettings
    )
    {
        _register = register;
        _storage = storage;
        _profile = profile;
        _organisations = organisations;
        _logger = logger;
        _generalSettings = generalSettings;
    }

    /// <summary>
    /// Presents the receipt of an instance.
    /// </summary>
    /// <param name="instanceOwnerId">The party id of the instance owner</param>
    /// <param name="instanceId">The instance guid</param>
    /// <param name="returnUrl">The URL the user should be sent to when leaving the receipt</param>
    /// <returns>The receipt page</returns>
    [HttpGet]
    [Route("receipt/{instanceOwnerId:int}/{instanceId:guid}")]
    public async Task<IActionResult> Index(int instanceOwnerId, Guid instanceId, [FromQuery] string returnUrl = null)
    {
        _logger.LogInformation(
            "Getting receipt for: {InstanceOwnerId} for instance with id: {InstanceId}.",
            instanceOwnerId,
            instanceId
        );

        UserProfile user;
        Instance instance;

        try
        {
            user = await GetUser();
            instance = await _storage.GetInstance(instanceOwnerId, instanceId);
        }
        catch (PlatformHttpException e)
        {
            return HandlePlatformHttpException(e);
        }

        string language = GetLanguage(user);
        (string org, string app) = SplitAppId(instance.AppId);

        Application application = null;
        TextResource textResource = null;

        if (org != null)
        {
            try
            {
                application = await _storage.GetApplication(org, app);
            }
            catch (PlatformHttpException e) when (e.Response.StatusCode != HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning(e, "Unable to retrieve application metadata for {AppId}.", instance.AppId);
            }
            catch (PlatformHttpException e)
            {
                return HandlePlatformHttpException(e);
            }

            textResource = await GetTextResource(org, app, language);
        }

        ReceiptPageContext context = new()
        {
            Instance = instance,
            InstanceGuid = instanceId,
            Party = await GetParty(instance),
            User = user,
            Application = application,
            TextResource = textResource,
            Organisations = await _organisations.GetOrgs(),
            Language = language,
            Host = Request.Host.Value,
            RequestedReturnUrl = returnUrl,
            AttachmentGroupsToHide = _generalSettings.Value.AttachmentGroupsToHide,
        };

        return View(ReceiptViewModelFactory.Create(context));
    }

    private async Task<UserProfile> GetUser()
    {
        string userIdString = User
            .Claims.Where(claim => claim.Type == AltinnCoreClaimTypes.UserId)
            .Select(claim => claim.Value)
            .FirstOrDefault();

        if (!int.TryParse(userIdString, out int userId))
        {
            // The receipt is also reachable with tokens that do not represent a user, for instance an organisation
            // token. The receipt is then presented without the name of the user.
            _logger.LogInformation("No user id in claims. Presenting the receipt without user information.");
            return null;
        }

        return await _profile.GetUser(userId);
    }

    private async Task<Party> GetParty(Instance instance)
    {
        if (!int.TryParse(instance.InstanceOwner?.PartyId, out int partyId))
        {
            return null;
        }

        try
        {
            return await _register.GetParty(partyId);
        }
        catch (PlatformHttpException e)
        {
            _logger.LogWarning(e, "Unable to retrieve party {PartyId}.", partyId);
            return null;
        }
    }

    private async Task<TextResource> GetTextResource(string org, string app, string language)
    {
        foreach (string candidate in ReceiptTexts.GetLanguagePriority(language))
        {
            try
            {
                TextResource textResource = await _storage.GetTextResource(org, app, candidate);
                if (textResource != null)
                {
                    return textResource;
                }
            }
            catch (PlatformHttpException e)
            {
                _logger.LogWarning(e, "Unable to retrieve {Language} texts for {Org}/{App}.", candidate, org, app);
                return null;
            }
        }

        return null;
    }

    private string GetLanguage(UserProfile user)
    {
        string language = LanguageHelper.GetLanguageFromAltinnPersistenceCookie(Request.Cookies[LanguageCookieName]);

        if (string.IsNullOrEmpty(language))
        {
            language = user?.ProfileSettingPreference?.Language;
        }

        return ReceiptTexts.SupportedLanguages.Contains(language) ? language : ReceiptTexts.SupportedLanguages[0];
    }

    private static (string Org, string App) SplitAppId(string appId)
    {
        string[] parts = appId?.Split('/') ?? [];

        return parts.Length == 2 ? (parts[0], parts[1]) : (null, null);
    }

    private IActionResult HandlePlatformHttpException(PlatformHttpException e)
    {
        switch (e.Response.StatusCode)
        {
            case HttpStatusCode.Unauthorized:
                // An empty response makes the status code pages middleware redirect the user to log in.
                return Unauthorized();
            case HttpStatusCode.Forbidden:
                return Error(HttpStatusCode.Forbidden, "error_no_access");
            case HttpStatusCode.NotFound:
                return Error(HttpStatusCode.NotFound, "error_not_found");
            default:
                _logger.LogError(e, "Unable to present the receipt.");
                return Error(HttpStatusCode.InternalServerError, "error_unknown");
        }
    }

    private IActionResult Error(HttpStatusCode statusCode, string messageKey)
    {
        string language = GetLanguage(null);
        IReadOnlyDictionary<string, string> texts = ReceiptTexts.GetDefaults(language);

        Response.StatusCode = (int)statusCode;

        return View(
            "Error",
            new ErrorViewModel
            {
                Language = language,
                Heading = texts["error_title"],
                Message = texts[messageKey],
            }
        );
    }
}
