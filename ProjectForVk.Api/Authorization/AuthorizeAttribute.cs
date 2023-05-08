using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjectForVk.Core.Entities.DB;

namespace ProjectForVk.Api.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous)
            return;

        var user = (UserEntity)context.HttpContext.Items["User"];
        
        if (user is null)
        {
            // not logged in - return 401 unauthorized
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };

            // set 'WWW-AuthenticateAsync' header to trigger login popup in browsers
            context.HttpContext.Response.Headers["WWW-AuthenticateAsync"] = "Basic realm=\"\", charset=\"UTF-8\"";
        }
    }
}