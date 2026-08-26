using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Altinn.Platform.Receipt.Controllers;
using Altinn.Platform.Receipt.Helpers;
using Altinn.Platform.Receipt.Model;
using Altinn.Platform.Receipt.Services.Interfaces;
using Altinn.Platform.Receipt.Tests.Mocks;
using Altinn.Platform.Receipt.Tests.Testdata;
using Altinn.Platform.Storage.Interface.Models;
using AltinnCore.Authentication.Constants;
using AltinnCore.Authentication.JwtCookie;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Altinn.Platform.Receipt.Tests;

public class ReceiptControllerTests : IClassFixture<WebApplicationFactory<ReceiptController>>
{
    private readonly WebApplicationFactory<ReceiptController> _factory;
    private readonly Mock<IRegister> _registerMock;
    private readonly Mock<IStorage> _storageMock;
    private readonly Mock<IProfile> _profileMock;
    private readonly Mock<IAltinnOrganisations> _organisationsMock;

    private static string ReceiptUrl => $"/receipt/{Parties.Party1.PartyId}/{Instances.InstanceGuid}";

    /// <summary>
    /// Initialises a new instance of the <see cref="ReceiptControllerTests"/> class with the given WebApplicationFactory.
    /// </summary>
    /// <param name="factory">The WebApplicationFactory to use when creating a test server.</param>
    public ReceiptControllerTests(WebApplicationFactory<ReceiptController> factory)
    {
        _factory = factory;
        _registerMock = new Mock<IRegister>();
        _storageMock = new Mock<IStorage>();
        _profileMock = new Mock<IProfile>();
        _organisationsMock = new Mock<IAltinnOrganisations>();

        _registerMock.Setup(register => register.GetParty(It.IsAny<int>())).ReturnsAsync(Parties.Party1);
        _storageMock
            .Setup(storage => storage.GetInstance(It.IsAny<int>(), It.IsAny<Guid>()))
            .ReturnsAsync(Instances.ArchivedInstance);
        _storageMock
            .Setup(storage => storage.GetApplication(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Applications.Application1);
        _storageMock
            .Setup(storage => storage.GetTextResource(It.IsAny<string>(), It.IsAny<string>(), "nb"))
            .ReturnsAsync(TextResources.Norwegian);
        _profileMock.Setup(profile => profile.GetUser(It.IsAny<int>())).ReturnsAsync(UserProfiles.User1);
        _organisationsMock
            .Setup(organisations => organisations.GetOrgs())
            .ReturnsAsync(new AltinnOrgs { Orgs = new Dictionary<string, AltinnOrg>() });
    }

    [Fact]
    public async Task Index_TC01_RendersTheReceipt()
    {
        HttpClient client = GetTestClient();

        HttpResponseMessage response = await client.GetAsync(ReceiptUrl);
        string html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType.MediaType);
        Assert.Contains("<title>Kvittering</title>", html);
        Assert.Contains("Testskjema er sendt inn", html);
        Assert.Contains("12345678901-Ola Nordmann", html);
        Assert.Contains("4164e925812b", html);
        Assert.Contains("Ola Nordmann", html);
    }

    [Fact]
    public async Task Index_TC02_RendersTheDesignSystemStylesheets()
    {
        HttpClient client = GetTestClient();

        string html = await client.GetStringAsync(ReceiptUrl);

        Assert.Contains("/receipt/css/designsystemet/theme.css", html);
        Assert.Contains("/receipt/css/designsystemet/components.css", html);
        Assert.Contains("/receipt/css/receipt.css", html);
        Assert.DoesNotContain("receipt.js", html);
    }

    [Fact]
    public async Task Index_TC03_RendersTheAttachments()
    {
        HttpClient client = GetTestClient();

        string html = await client.GetStringAsync(ReceiptUrl);

        Assert.Contains("Følgende er sendt inn:", html);
        Assert.Contains("skjema.pdf", html);
        Assert.Contains("Vedlegg (1)", html);
        Assert.Contains("vedlegg.pdf", html);
        Assert.Contains("Andre vedlegg (1)", html);
        Assert.Contains("gruppert.pdf", html);
        Assert.DoesNotContain("skjult.pdf", html);
    }

    [Fact]
    public async Task Index_TC04_LanguageFromCookie_RendersEnglishTexts()
    {
        _storageMock
            .Setup(storage => storage.GetTextResource(It.IsAny<string>(), It.IsAny<string>(), "en"))
            .ReturnsAsync((TextResource)null);

        HttpClient client = GetTestClient();
        client.DefaultRequestHeaders.Add("Cookie", "altinnPersistentContext=UL=1033");

        string html = await client.GetStringAsync(ReceiptUrl);

        Assert.Contains("<html lang=\"en\"", html);
        Assert.Contains("<title>Receipt</title>", html);
        Assert.Contains("Reference number", html);
    }

    [Fact]
    public async Task Index_TC05_Substatus_IsRendered()
    {
        Instance instance = Instances.ArchivedInstance;
        instance.Status.Substatus = new Substatus { Label = "Til behandling", Description = "Saken er mottatt" };
        _storageMock.Setup(storage => storage.GetInstance(It.IsAny<int>(), It.IsAny<Guid>())).ReturnsAsync(instance);

        HttpClient client = GetTestClient();

        string html = await client.GetStringAsync(ReceiptUrl);

        Assert.Contains("Til behandling", html);
        Assert.Contains("Saken er mottatt", html);
    }

    [Fact]
    public async Task Index_TC06_NoToken_RedirectsToAuthentication()
    {
        HttpClient client = GetTestClient(allowRedirects: false);
        client.DefaultRequestHeaders.Authorization = null;

        HttpResponseMessage response = await client.GetAsync(ReceiptUrl);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("authentication?goto=", response.Headers.Location.ToString());
    }

    [Fact]
    public async Task Index_TC07_InstanceNotFound_RendersAnErrorPage()
    {
        _storageMock
            .Setup(storage => storage.GetInstance(It.IsAny<int>(), It.IsAny<Guid>()))
            .ThrowsAsync(
                new PlatformHttpException(
                    new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound },
                    string.Empty
                )
            );

        HttpClient client = GetTestClient();

        HttpResponseMessage response = await client.GetAsync(ReceiptUrl);
        string html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("Vi finner ikke denne kvitteringen.", html);
    }

    [Fact]
    public async Task Index_TC08_NoAccessToInstance_RendersAnErrorPage()
    {
        _storageMock
            .Setup(storage => storage.GetInstance(It.IsAny<int>(), It.IsAny<Guid>()))
            .ThrowsAsync(
                new PlatformHttpException(
                    new HttpResponseMessage { StatusCode = HttpStatusCode.Forbidden },
                    string.Empty
                )
            );

        HttpClient client = GetTestClient();

        HttpResponseMessage response = await client.GetAsync(ReceiptUrl);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Contains("Du har ikke tilgang til denne kvitteringen.", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Index_TC09_ApplicationMetadataUnavailable_StillRendersTheReceipt()
    {
        _storageMock
            .Setup(storage => storage.GetApplication(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(
                new PlatformHttpException(
                    new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError },
                    string.Empty
                )
            );

        HttpClient client = GetTestClient();

        HttpResponseMessage response = await client.GetAsync(ReceiptUrl);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Testskjema er sendt inn", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Index_TC10_UnknownInstanceOwner_ReturnsNotFound()
    {
        HttpClient client = GetTestClient();

        HttpResponseMessage response = await client.GetAsync("/receipt/not-a-party/not-a-guid");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static string GetUserToken(int userId)
    {
        List<Claim> claims = new();
        string issuer = "www.altinn.no";

        claims.Add(new Claim(AltinnCoreClaimTypes.UserId, userId.ToString(), ClaimValueTypes.String, issuer));
        claims.Add(new Claim(AltinnCoreClaimTypes.UserName, "UserOne", ClaimValueTypes.String, issuer));
        claims.Add(
            new Claim(AltinnCoreClaimTypes.PartyID, (userId + 5000).ToString(), ClaimValueTypes.Integer32, issuer)
        );
        claims.Add(new Claim(AltinnCoreClaimTypes.AuthenticateMethod, "Mock", ClaimValueTypes.String, issuer));
        claims.Add(new Claim(AltinnCoreClaimTypes.AuthenticationLevel, "2", ClaimValueTypes.Integer32, issuer));

        ClaimsIdentity identity = new("mock");
        identity.AddClaims(claims);

        return JwtTokenMock.GenerateToken(new ClaimsPrincipal(identity));
    }

    private HttpClient GetTestClient(bool allowRedirects = true)
    {
        string configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

        HttpClient client = _factory
            .WithWebHostBuilder(builder =>
            {
                builder
                    .ConfigureTestServices(services =>
                    {
                        services.AddSingleton(_registerMock.Object);
                        services.AddSingleton(_storageMock.Object);
                        services.AddSingleton(_profileMock.Object);
                        services.AddSingleton(_organisationsMock.Object);
                        services.AddSingleton<
                            IPostConfigureOptions<JwtCookieOptions>,
                            JwtCookiePostConfigureOptionsStub
                        >();
                    })
                    .ConfigureAppConfiguration((context, conf) => conf.AddJsonFile(configPath));
            })
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = allowRedirects });

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetUserToken(1));

        return client;
    }
}
