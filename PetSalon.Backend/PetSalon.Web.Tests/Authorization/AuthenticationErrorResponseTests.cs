using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using PetSalon.Web.Authorization;

namespace PetSalon.Web.Tests.Authorization;

public sealed class AuthenticationErrorResponseTests
{
    [Theory]
    [InlineData(AuthenticationFailureKind.Unauthorized, StatusCodes.Status401Unauthorized, "Unauthorized")]
    [InlineData(AuthenticationFailureKind.Forbidden, StatusCodes.Status403Forbidden, "Forbidden")]
    public async Task AuthenticationFailuresUseSanitizedProblemDetails(
        AuthenticationFailureKind kind,
        int expectedStatus,
        string expectedTitle)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await AuthenticationErrorResponseWriter.WriteAsync(context, kind);

        Assert.Equal(expectedStatus, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);
        context.Response.Body.Position = 0;
        using var document = await JsonDocument.ParseAsync(context.Response.Body);
        var root = document.RootElement;
        Assert.Equal(expectedStatus, root.GetProperty("status").GetInt32());
        Assert.Equal(expectedTitle, root.GetProperty("title").GetString());
        Assert.False(root.TryGetProperty("detail", out _));
        var body = root.GetRawText().ToLowerInvariant();
        Assert.DoesNotContain("password", body);
        Assert.DoesNotContain("token", body);
        Assert.DoesNotContain("hash", body);
        Assert.DoesNotContain("secret", body);
        Assert.DoesNotContain("exception", body);
    }

    [Fact]
    public async Task ResponseWriterDoesNotOverwriteAStartedResponse()
    {
        var features = new FeatureCollection();
        var response = new StartedResponseFeature();
        features.Set<IHttpResponseFeature>(response);
        var context = new DefaultHttpContext(features);

        await AuthenticationErrorResponseWriter.WriteAsync(
            context,
            AuthenticationFailureKind.Unauthorized);

        Assert.NotEqual(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        Assert.Equal(0, response.Body.Length);
    }

    private sealed class StartedResponseFeature : IHttpResponseFeature
    {
        public int StatusCode { get; set; } = StatusCodes.Status200OK;
        public string? ReasonPhrase { get; set; }
        public IHeaderDictionary Headers { get; set; } = new HeaderDictionary();
        public Stream Body { get; set; } = new MemoryStream();
        public bool HasStarted => true;

        public void OnStarting(Func<object, Task> callback, object state)
        {
        }

        public void OnCompleted(Func<object, Task> callback, object state)
        {
        }
    }
}
