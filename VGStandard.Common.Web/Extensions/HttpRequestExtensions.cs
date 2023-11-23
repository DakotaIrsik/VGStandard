using Microsoft.AspNetCore.Http;

namespace VGStandard.Common.Web.Extensions;

public static class HttpRequestExtensions
{
    public static bool IsRobotsTxt(this HttpRequest? request)
    {
        return request?.Path.Value?.EndsWith("robots.txt") ?? false;
    }
}
