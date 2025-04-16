using AspNetCoreStudy.Middleware;

namespace AspNetCoreStudy;

public static class WebApplicationExt
{
    public static void AddMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<SessionMiddleware>();
    }
}
