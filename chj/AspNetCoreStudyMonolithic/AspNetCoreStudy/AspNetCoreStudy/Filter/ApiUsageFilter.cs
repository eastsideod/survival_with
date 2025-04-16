namespace AspNetCoreStudy.Filter;

using AspNetCoreStudy.BackService.UsageMonintor;
using Microsoft.AspNetCore.Mvc.Filters;

public class ApiUsageFilter : IActionFilter
{
    private ApiUsageCounter usageCounter;

    public ApiUsageFilter(ApiUsageCounter usageCounter)
    {
        this.usageCounter = usageCounter;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        this.usageCounter.Increment();
    }
}
