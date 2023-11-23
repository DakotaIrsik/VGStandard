using Microsoft.AspNetCore.Builder;
using VGStandard.Common.Web.Middleware;

namespace VGStandard.Common.Web.Extensions
{
    public static class ProtectFolderExtensions
    {
        public static IApplicationBuilder UseProtectFolder(
                this IApplicationBuilder builder,
                ProtectFolderOptions options)
        {
            return builder.UseMiddleware<ProtectFolder>(options);
        }
    }
}
