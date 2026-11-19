using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Altinn.Platform.Receipt.Tests.Mocks;

/// <summary>
/// Message handler that records each request and answers with a fixed response.
/// </summary>
public class RecordingHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage _response;

    public RecordingHttpMessageHandler(HttpResponseMessage response = null)
    {
        _response = response ?? new HttpResponseMessage(HttpStatusCode.OK);
    }

    public List<HttpRequestMessage> Requests { get; } = [];

    public HttpRequestMessage LastRequest => Requests[^1];

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        Requests.Add(request);
        return Task.FromResult(_response);
    }
}
