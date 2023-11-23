using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using VGStandard.Common.Web.Extensions;

namespace VGStandard.Common.Web.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Stopwatch sw;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
            sw = new Stopwatch();
        }

        public async Task Invoke(HttpContext context, IWebHostEnvironment env, ILogger<ExceptionHandlingMiddleware> logger)
        {
            try
            {
                sw.Reset();
                sw.Start();
                await _next(context);
                sw.Stop();
                //logger.LogWarning($"Execution of method {context.GetEndpoint().DisplayName} took {sw.ElapsedMilliseconds} milliseconds to complete");
            }
            catch (Exception ex)
            {
                 HandleExceptionAsync(context, ex, env, logger);
            }
        }

        //https://stackoverflow.com/questions/38630076/asp-net-core-web-api-exception-handling
        private static void HandleExceptionAsync(HttpContext context, Exception exception, IWebHostEnvironment env, ILogger logger)
        {
            var requestData = new Dictionary<string, string>
            {
                ["RequestPath"] = context.Request.Path.Value,
                ["RequestQueryString"] = context.Request.QueryString.Value,
            };

            logger.LogError(exception, "Unhandled Exception", requestData);

            string result;
            if (!env.IsProduction())
            {
                result = JsonConvert.SerializeObject(new
                {
                    Error = exception.Message,
                    exception.StackTrace,
                    Name = "Unhandled exception: " + exception.GetType().Name,
                    exception?.InnerException
                });
            }
            else
            {
                result = JsonConvert.SerializeObject(new
                {
                    Errors = new Dictionary<string, string> { ["Error"] = "An error occurred. Please target a lower environment to see more details." }
                });
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;
            context.Response.WriteAsync(result);
            context.Response.CompleteAsync();
        }
    }
}

