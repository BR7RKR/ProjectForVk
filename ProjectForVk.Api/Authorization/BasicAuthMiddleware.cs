using System.Net.Http.Headers;
using System.Text;
using ProjectForVk.Application.Services;

namespace ProjectForVk.Api.Authorization;

public sealed class BasicAuthMiddleware
{
    private readonly RequestDelegate _next;

    public BasicAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IUserService userService)
    {
        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(context.Request.Headers["Authorization"]);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter ?? throw new InvalidOperationException());
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
            var username = credentials[0];
            var password = credentials[1];
            
            context.Items["User"] = await userService.AuthenticateAsync(username, password);
        }
        catch
        {
            // ignored
        }

        await _next(context);
    }
}