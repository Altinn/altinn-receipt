using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Altinn.Platform.Receipt.Helpers;
using Altinn.Platform.Receipt.Services.Interfaces;
using Altinn.Platform.Receipt.Tests.Mocks;
using Altinn.Platform.Receipt.Tests.Testdata;
using Altinn.Platform.Storage.Interface.Models;
using Xunit;

namespace Altinn.Platform.Receipt.Tests.Services;

public class StorageWrapperTests
{
    private const int InstanceOwnerPartyId = 1000;
    private static readonly Guid InstanceGuid = Guid.Parse("1c3a4b9d-cbbe-4146-b370-4164e925812b");

    [Fact]
    public void Constructor_ConfiguresBaseAddressAndDefaultHeaders()
    {
        HttpClient client = new(new RecordingHttpMessageHandler());

        _ = CreateWrapper(client);

        Assert.Equal(WrapperTestSetup.StorageEndpoint, client.BaseAddress.ToString());
        Assert.Equal(
            WrapperTestSetup.SubscriptionKey,
            client.DefaultRequestHeaders.GetValues("Ocp-Apim-Subscription-Key").Single()
        );
        Assert.Contains(client.DefaultRequestHeaders.Accept, accept => accept.MediaType == "application/json");
    }

    [Fact]
    public async Task GetInstance_Ok_ReturnsInstanceAndSendsUserToken()
    {
        Instance expected = Instances.Instance1;
        RecordingHttpMessageHandler handler = new(WrapperTestSetup.JsonResponse(expected));
        StorageWrapper wrapper = CreateWrapper(new HttpClient(handler));

        Instance instance = await wrapper.GetInstance(InstanceOwnerPartyId, InstanceGuid);

        Assert.Equal(expected.Id, instance.Id);
        Assert.Equal(expected.AppId, instance.AppId);
        Assert.Equal(expected.InstanceOwner.PartyId, instance.InstanceOwner.PartyId);
        Assert.Equal(expected.Process.CurrentTask.ElementId, instance.Process.CurrentTask.ElementId);

        HttpRequestMessage request = handler.LastRequest;
        Assert.Equal(HttpMethod.Get, request.Method);
        Assert.Equal(
            $"{WrapperTestSetup.StorageEndpoint}instances/{InstanceOwnerPartyId}/{InstanceGuid}",
            request.RequestUri.ToString()
        );
        Assert.Equal(WrapperTestSetup.UserToken, request.Headers.Authorization?.Parameter);
        Assert.False(request.Headers.Contains("PlatformAccessToken"));
    }

    [Fact]
    public async Task GetInstance_NotOk_ThrowsPlatformHttpException()
    {
        HttpResponseMessage response = new(HttpStatusCode.InternalServerError);
        StorageWrapper wrapper = CreateWrapper(new HttpClient(new RecordingHttpMessageHandler(response)));

        PlatformHttpException exception = await Assert.ThrowsAsync<PlatformHttpException>(() =>
            wrapper.GetInstance(InstanceOwnerPartyId, InstanceGuid)
        );

        Assert.Same(response, exception.Response);
    }

    private static StorageWrapper CreateWrapper(HttpClient client) =>
        new(client, WrapperTestSetup.HttpContextAccessor(), WrapperTestSetup.PlatformSettings());
}
