using AspNetCoreStudy.BackService.UsageMonintor;
using AspNetCoreStudy.Filter;
using AspNetCoreStudy.Service;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreStudy;

public static class IServiceCollectionExt
{
    public static void AddConfigures(this IServiceCollection services)
    {
    }

    public static void AddSystemServices(this IServiceCollection services)
    {
        services.AddSingleton<ApiUsageCounter>();
    }

    public static void AddOutGameServices(this IServiceCollection services)
    {
        services.AddSingleton<SessionService>();
        services.AddSingleton<AccountService>();
    }

    public static void AddHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<UsageMonitorService>();
    }

    public static void AddFilters(this IServiceCollection services)
    {
        services.Configure<MvcOptions>(options =>
        {
            options.Filters.Add<ExceptionFilter>();
            options.Filters.Add<ApiUsageFilter>();
        });
    }
}
