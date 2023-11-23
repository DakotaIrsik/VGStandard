using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Text;
using System.Threading.Tasks;

namespace VGStandard.Common.Web.Middleware
{
	public class RobotHandlingMiddleware
	{
		private readonly IHostEnvironment _env;

		public RobotHandlingMiddleware(RequestDelegate request, IHostEnvironment env)
		{
			_env = env;
		}

		public async Task Invoke(HttpContext context)
		{
			var sb = new StringBuilder();


				sb
					.AppendLine("User-agent: *")
					.AppendLine("Disallow: / ");

			context.Response.StatusCode = 200;
			context.Response.ContentType = "text/plain";
			await context.Response.WriteAsync(sb.ToString());
		}
	}
}
