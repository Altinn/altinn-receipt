using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Altinn.Platform.Receipt.Extensions;
using Altinn.Platform.Receipt.Tests.Mocks;
using Xunit;

namespace Altinn.Platform.Receipt.Tests.Extensions;

public class HttpClientExtensionTests
{
    private const string UserToken = "user-token";
    private const string PlatformAccessToken = "platform-access-token";
    private const string RequestUri = "resource/1";

    private readonly RecordingHttpMessageHandler _handler = new();
    private readonly HttpClient _client;

    public HttpClientExtensionTests()
    {
        _client = new HttpClient(_handler) { BaseAddress = new Uri("http://platform.test/api/v1/") };
    }

    [Fact]
    public async Task GetAsync_SendsGetWithBearerAndPlatformAccessToken()
    {
        await _client.GetAsync(UserToken, RequestUri, PlatformAccessToken);

        HttpRequestMessage request = _handler.LastRequest;
        Assert.Equal(HttpMethod.Get, request.Method);
        Assert.Equal("http://platform.test/api/v1/resource/1", request.RequestUri.ToString());
        AssertBearerToken(request);
        Assert.Equal(PlatformAccessToken, request.Headers.GetValues("PlatformAccessToken").Single());
    }

    [Fact]
    public async Task PostAsync_SendsPostWithContentAndPlatformAccessToken()
    {
        using StringContent content = new("body");

        await _client.PostAsync(UserToken, RequestUri, content, PlatformAccessToken);

        HttpRequestMessage request = _handler.LastRequest;
        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.Same(content, request.Content);
        AssertBearerToken(request);
        Assert.Equal(PlatformAccessToken, request.Headers.GetValues("PlatformAccessToken").Single());
    }

    [Fact]
    public async Task PutAsync_SendsPutWithContentAndPlatformAccessToken()
    {
        using StringContent content = new("body");

        await _client.PutAsync(UserToken, RequestUri, content, PlatformAccessToken);

        HttpRequestMessage request = _handler.LastRequest;
        Assert.Equal(HttpMethod.Put, request.Method);
        Assert.Same(content, request.Content);
        AssertBearerToken(request);
        Assert.Equal(PlatformAccessToken, request.Headers.GetValues("PlatformAccessToken").Single());
    }

    [Fact]
    public async Task DeleteAsync_SendsDeleteWithPlatformAccessToken()
    {
        await _client.DeleteAsync(UserToken, RequestUri, PlatformAccessToken);

        HttpRequestMessage request = _handler.LastRequest;
        Assert.Equal(HttpMethod.Delete, request.Method);
        AssertBearerToken(request);
        Assert.Equal(PlatformAccessToken, request.Headers.GetValues("PlatformAccessToken").Single());
    }

    [Fact]
    public async Task Requests_WithoutPlatformAccessToken_OmitPlatformAccessTokenHeader()
    {
        using StringContent postContent = new("body");
        using StringContent putContent = new("body");

        await _client.GetAsync(UserToken, RequestUri);
        await _client.PostAsync(UserToken, RequestUri, postContent);
        await _client.PutAsync(UserToken, RequestUri, putContent);
        await _client.DeleteAsync(UserToken, RequestUri);

        Assert.Equal(4, _handler.Requests.Count);
        Assert.All(
            _handler.Requests,
            request =>
            {
                AssertBearerToken(request);
                Assert.False(request.Headers.Contains("PlatformAccessToken"));
            }
        );
    }

    private static void AssertBearerToken(HttpRequestMessage request)
    {
        Assert.Equal("Bearer", request.Headers.Authorization?.Scheme);
        Assert.Equal(UserToken, request.Headers.Authorization?.Parameter);
    }
}
