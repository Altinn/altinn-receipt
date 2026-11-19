using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Altinn.Common.AccessTokenClient.Services;
using Altinn.Platform.Profile.Models;
using Altinn.Platform.Receipt.Helpers;
using Altinn.Platform.Receipt.Services.Interfaces;
using Altinn.Platform.Receipt.Tests.Mocks;
using Altinn.Platform.Receipt.Tests.Testdata;
using Moq;
using Xunit;

namespace Altinn.Platform.Receipt.Tests.Services;

public class ProfileWrapperTests
{
    private const string PlatformAccessToken = "platform-access-token";

    private readonly Mock<IAccessTokenGenerator> _accessTokenGenerator = new();

    public ProfileWrapperTests()
    {
        _accessTokenGenerator.Setup(g => g.GenerateAccessToken("platform", "receipt")).Returns(PlatformAccessToken);
    }

    [Fact]
    public void Constructor_ConfiguresBaseAddressAndDefaultHeaders()
    {
        HttpClient client = new(new RecordingHttpMessageHandler());

        _ = CreateWrapper(client);

        Assert.Equal(WrapperTestSetup.ProfileEndpoint, client.BaseAddress.ToString());
        Assert.Equal(
            WrapperTestSetup.SubscriptionKey,
            client.DefaultRequestHeaders.GetValues("Ocp-Apim-Subscription-Key").Single()
        );
        Assert.Contains(client.DefaultRequestHeaders.Accept, accept => accept.MediaType == "application/json");
    }

    [Fact]
    public async Task GetUser_Ok_ReturnsUserProfileAndSendsTokens()
    {
        UserProfile expected = UserProfiles.User1;
        RecordingHttpMessageHandler handler = new(WrapperTestSetup.JsonResponse(expected));
        ProfileWrapper wrapper = CreateWrapper(new HttpClient(handler));

        UserProfile user = await wrapper.GetUser(expected.UserId);

        Assert.Equal(expected.UserId, user.UserId);
        Assert.Equal(expected.PartyId, user.PartyId);
        Assert.Equal(expected.UserType, user.UserType);

        HttpRequestMessage request = handler.LastRequest;
        Assert.Equal(HttpMethod.Get, request.Method);
        Assert.Equal($"{WrapperTestSetup.ProfileEndpoint}users/{expected.UserId}", request.RequestUri.ToString());
        Assert.Equal(WrapperTestSetup.UserToken, request.Headers.Authorization?.Parameter);
        Assert.Equal(PlatformAccessToken, request.Headers.GetValues("PlatformAccessToken").Single());
    }

    [Fact]
    public async Task GetUser_NotOk_ThrowsPlatformHttpException()
    {
        HttpResponseMessage response = new(HttpStatusCode.Forbidden);
        ProfileWrapper wrapper = CreateWrapper(new HttpClient(new RecordingHttpMessageHandler(response)));

        PlatformHttpException exception = await Assert.ThrowsAsync<PlatformHttpException>(() => wrapper.GetUser(1));

        Assert.Same(response, exception.Response);
    }

    private ProfileWrapper CreateWrapper(HttpClient client) =>
        new(
            client,
            WrapperTestSetup.HttpContextAccessor(),
            WrapperTestSetup.PlatformSettings(),
            _accessTokenGenerator.Object
        );
}
