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

            if (requestPath.StartsWithSegments("/api/User/login"))
            {
                await _next.Invoke(context);
                return;
            }

            var userIdentity = context.User.Identity as ClaimsIdentity;
            if (userIdentity is { IsAuthenticated: false })
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            var user = await userRepo.GetUserById(userIdentity?.FindFirst("userid")?.Value);

            if (user.Status.Equals(Status.Inactive))
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