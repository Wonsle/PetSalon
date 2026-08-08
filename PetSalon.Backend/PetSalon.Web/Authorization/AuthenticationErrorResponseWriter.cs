using System.Text.Json;

namespace PetSalon.Web.Authorization;

public enum AuthenticationFailureKind
{
    Unauthorized,
    Forbidden
}

public static class AuthenticationErrorResponseWriter
{
    public static async Task WriteAsync(
        HttpContext context,
        AuthenticationFailureKind failureKind)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        var (status, title) = failureKind switch
        {
            AuthenticationFailureKind.Unauthorized =>
                (StatusCodes.Status401Unauthorized, "Unauthorized"),
            AuthenticationFailureKind.Forbidden =>
                (StatusCodes.Status403Forbidden, "Forbidden"),
            _ => throw new ArgumentOutOfRangeException(nameof(failureKind))
        };

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        await JsonSerializer.SerializeAsync(
            context.Response.Body,
            new { type = $"https://httpstatuses.com/{status}", title, status },
            cancellationToken: context.RequestAborted);
    }
}
