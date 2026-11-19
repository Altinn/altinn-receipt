using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Altinn.Common.AccessTokenClient.Services;
using Altinn.Platform.Receipt.Helpers;
using Altinn.Platform.Receipt.Services;
using Altinn.Platform.Receipt.Tests.Mocks;
using Altinn.Platform.Receipt.Tests.Testdata;
using Altinn.Platform.Register.Models;
using Moq;
using Xunit;

namespace Altinn.Platform.Receipt.Tests.Services;

public class RegisterWrapperTests
{
    private const string PlatformAccessToken = "platform-access-token";

    private readonly Mock<IAccessTokenGenerator> _accessTokenGenerator = new();

    public RegisterWrapperTests()
    {
        _accessTokenGenerator.Setup(g => g.GenerateAccessToken("platform", "receipt")).Returns(PlatformAccessToken);
    }

    [Fact]
    public void Constructor_ConfiguresBaseAddressAndDefaultHeaders()
    {
        HttpClient client = new(new RecordingHttpMessageHandler());

        _ = CreateWrapper(client);

        Assert.Equal(WrapperTestSetup.RegisterEndpoint, client.BaseAddress.ToString());
        Assert.Equal(
            WrapperTestSetup.SubscriptionKey,
            client.DefaultRequestHeaders.GetValues("Ocp-Apim-Subscription-Key").Single()
        );
        Assert.Contains(client.DefaultRequestHeaders.Accept, accept => accept.MediaType == "application/json");
    }

    [Fact]
    public async Task GetParty_Ok_ReturnsPartyAndSendsTokens()
    {
        Party expected = Parties.Party1;
        RecordingHttpMessageHandler handler = new(WrapperTestSetup.JsonResponse(expected));
        RegisterWrapper wrapper = CreateWrapper(new HttpClient(handler));

        Party party = await wrapper.GetParty(expected.PartyId);

        Assert.Equal(expected.PartyId, party.PartyId);
        Assert.Equal(expected.SSN, party.SSN);
        Assert.Equal(expected.PartyTypeName, party.PartyTypeName);

        HttpRequestMessage request = handler.LastRequest;
        Assert.Equal(HttpMethod.Get, request.Method);
        Assert.Equal($"{WrapperTestSetup.RegisterEndpoint}parties/{expected.PartyId}", request.RequestUri.ToString());
        Assert.Equal(WrapperTestSetup.UserToken, request.Headers.Authorization?.Parameter);
        Assert.Equal(PlatformAccessToken, request.Headers.GetValues("PlatformAccessToken").Single());
    }

    [Fact]
    public async Task GetParty_NotOk_ThrowsPlatformHttpException()
    {
        HttpResponseMessage response = new(HttpStatusCode.NotFound);
        RegisterWrapper wrapper = CreateWrapper(new HttpClient(new RecordingHttpMessageHandler(response)));

        PlatformHttpException exception = await Assert.ThrowsAsync<PlatformHttpException>(() =>
            wrapper.GetParty(50001)
        );

        Assert.Same(response, exception.Response);
    }

    private RegisterWrapper CreateWrapper(HttpClient client) =>
        new(
            client,
            WrapperTestSetup.HttpContextAccessor(),
            WrapperTestSetup.PlatformSettings(),
            _accessTokenGenerator.Object
        );
}
