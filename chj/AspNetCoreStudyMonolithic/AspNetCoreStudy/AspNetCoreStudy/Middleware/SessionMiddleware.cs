using AspNetCoreStudy.Attribute;
using AspNetCoreStudy.Service;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreStudy.Middleware;

public class SessionMiddleware
{
    private readonly RequestDelegate _next;
    private SessionService sessionService;


    public SessionMiddleware(RequestDelegate next,
                             SessionService sessionService)
    {
        this._next = next;
        this.sessionService = sessionService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requset = context.Request;
        var endpoint = context.GetEndpoint();
        var needSkip = endpoint!.Metadata.OfType<SkipSessionFilter>().Any();

        if (needSkip == false)
        {
            if (requset.Headers.TryGetValue("Account-Id", out var accountId) == false)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized-1");
                return;
            }

            if (requset.Headers.TryGetValue("Session-Id", out var sessionId) == false)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized-2");
                return;
            }

            if (sessionService.Verify(accountId.ToString(), sessionId.ToString()) == false)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized-3");
                return;
            }
        }

        await this._next(context);
    }
}
