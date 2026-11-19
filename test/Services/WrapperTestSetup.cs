using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Altinn.Platform.Receipt.Configuration;
using Altinn.Platform.Receipt.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;

namespace Altinn.Platform.Receipt.Tests.Services;

/// <summary>
/// Shared setup for the tests of the platform service wrappers.
/// </summary>
internal static class WrapperTestSetup
{
    public const string UserToken = "user-token";
    public const string SubscriptionKey = "subscription-key";
    public const string RegisterEndpoint = "http://register.test/register/api/v1/";
    public const string ProfileEndpoint = "http://profile.test/profile/api/v1/";
    public const string StorageEndpoint = "http://storage.test/storage/api/v1/";

    public static IOptions<PlatformSettings> PlatformSettings() =>
        Options.Create(
            new PlatformSettings
            {
                ApiRegisterEndpoint = RegisterEndpoint,
                ApiProfileEndpoint = ProfileEndpoint,
                ApiStorageEndpoint = StorageEndpoint,
                SubscriptionKey = SubscriptionKey,
            }
        );

    /// <summary>
    /// Creates an accessor for a request that carries the user's token in the AltinnStudioRuntime cookie.
    /// </summary>
    public static IHttpContextAccessor HttpContextAccessor()
    {
        DefaultHttpContext context = new();
        context.Request.Headers.Cookie = $"AltinnStudioRuntime={UserToken}";

        Mock<IHttpContextAccessor> accessor = new();
        accessor.Setup(a => a.HttpContext).Returns(context);
        return accessor.Object;
    }

    public static HttpResponseMessage JsonResponse<T>(T body) =>
        new(HttpStatusCode.OK) { Content = JsonContent.Create(body, options: JsonSerializerOptionsProvider.Options) };
}
