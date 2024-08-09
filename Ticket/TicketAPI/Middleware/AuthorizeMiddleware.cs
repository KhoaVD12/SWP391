using System.Net;
using System.Security.Claims;
using DataAccessObject.Enums;
using DataAccessObject.IRepo;

namespace TicketAPI.Middleware;

public class AuthorizeMiddleware
{
    private readonly RequestDelegate _next;

    public AuthorizeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IUserRepo userRepo)
    {
        try
        {
            var requestPath = context.Request.Path;

            // Bypass middleware for login and CreateStaff endpoints
            if (requestPath.StartsWithSegments("/api/User/login") ||
                requestPath.StartsWithSegments("/api/User/staff")) 
            {
                await _next.Invoke(context);
                return;
            }

            var userIdentity = context.User.Identity as ClaimsIdentity;
            if (userIdentity == null || !userIdentity.IsAuthenticated)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            var userIdClaim = userIdentity.FindFirst("userid")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            var user = await userRepo.GetUserById(userIdClaim);

            if (user == null || user.Status.Equals(Status.Inactive))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync(ex.ToString());
        }
    }
}