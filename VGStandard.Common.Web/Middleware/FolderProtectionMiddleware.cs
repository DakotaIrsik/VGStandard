using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace VGStandard.Common.Web.Middleware
{
    public class ProtectFolderOptions
    {
        public PathString Path { get; set; }
        public string PolicyName { get; set; }
    }

    public class ProtectFolder
    {
        private readonly RequestDelegate _next;
        private readonly PathString _path;
        private readonly string _policyName;

        public ProtectFolder(RequestDelegate next, ProtectFolderOptions options)
        {
            _next = next;
            _path = options.Path;
            _policyName = options.PolicyName;
        }

        public async Task Invoke(HttpContext httpContext,
                                 IAuthorizationService authorizationService)
        {
            if (httpContext.Request.Path.StartsWithSegments(_path))
            {
                var authorized = await authorizationService.AuthorizeAsync(
                                    httpContext.User, null, _policyName);
                if (!authorized.Succeeded)
                {
                    await httpContext.Request.HttpContext.ChallengeAsync();
                    return;
                }
            }

            await _next(httpContext);
        }
    }
}
