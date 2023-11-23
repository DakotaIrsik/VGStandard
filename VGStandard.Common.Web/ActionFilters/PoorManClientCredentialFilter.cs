namespace VGStandard.Common.Web.ActionFilters;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

public class PoorManClientCredentialFilter : IAsyncActionFilter
{
    private readonly string _expectedToken;
    private readonly IWebHostEnvironment _environment;

    public PoorManClientCredentialFilter(string expectedToken, IWebHostEnvironment environment)
    {
        _expectedToken = expectedToken;
        _environment = environment;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (!authHeader.ToString().StartsWith("Bearer "))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var token = authHeader.ToString().Substring("Bearer ".Length).Trim();

        if (token != _expectedToken)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        await next();
    }
}
