using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using VGStandard.Core.Utility;

namespace VGStandard.Core.Telemetry
{
    public class OpenTelemetryInstrumentation
    {
        // httpRequestExpressionEvaluators is a map of attribute name to C# expression evaluator.
        Dictionary<string, Func<HttpRequest, object>> httpRequestExpressionEvaluators;
        // httpResponseExpressionEvaluators is a map of attribute name to C# expression evaluator.
        Dictionary<string, Func<HttpResponse, object>> httpResponseExpressionEvaluators;
        // httpRequestMessageExpressionEvaluators is a map of attribute name to C# expression evaluator.
        Dictionary<string, Func<HttpRequestMessage, object>> httpRequestMessageExpressionEvaluators;
        // httpResponseMessageExpressionEvaluators is a map of attribute name to C# expression evaluator.
        Dictionary<string, Func<HttpResponseMessage, object>> httpResponseMessageExpressionEvaluators;

        public OpenTelemetryInstrumentation()
        {
            httpRequestExpressionEvaluators = new Dictionary<string, Func<HttpRequest, object>>();
            httpResponseExpressionEvaluators = new Dictionary<string, Func<HttpResponse, object>>();
            httpRequestMessageExpressionEvaluators = new Dictionary<string, Func<HttpRequestMessage, object>>();
            httpResponseMessageExpressionEvaluators = new Dictionary<string, Func<HttpResponseMessage, object>>();
        }

        public void Configure(IServiceCollection services, IConfigurationSection config)
        {
            Console.WriteLine("Startup.cs [OpenTelemetryInstrumentation.Configure] - enabling OpenTelemetry");

            // OpenTelemetry:Instrumentation:AspNetCore:HttpRequestFields consists of a map of attribute name to C# expression.
            // The expression is evaluated against the HttpRequest object and the result is set as the attribute value.
            // For example, to set the attribute "userId" to the value of the User.Identity.Name property, add the following to appsettings.json:
            // "OpenTelemetry:Instrumentation:AspNetCore:HttpRequestFields": {
            //     "userId": "HttpContext.User?.Identity?.Name"
            // }
            foreach (var keyValue in config.GetSection("OpenTelemetry:Instrumentation:AspNetCore:HttpRequestFields").GetChildren())
            {
                try
                {
                    httpRequestExpressionEvaluators.Add(keyValue.Key, ExpressionEvaluatorCompiler<HttpRequest>.Compile(keyValue.Value) ?? throw new Exception("ExpressionEvaluatorCompiler<HttpRequest>.Compile returned null"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"OpenTelemetry.cs [OpenTelemetryInstrumentation constructor] - failed to compile expression '{keyValue.Value}' for attribute '{keyValue.Key}': {ex.GetFullMessage()}");
                    throw;
                }
            }
            // OpenTelemetry:Instrumentation:AspNetCore:HttpResponseFields consists of a map of attribute name to C# expression.
            // The expression is evaluated against the HttpResponse object and the result is set as the attribute value.
            // For example, to set the attribute "statusCode" to the value of the StatusCode property, add the following to appsettings.json:
            // "OpenTelemetry:Instrumentation:AspNetCore:HttpResponseFields": {
            //     "statusCode": "StatusCode"
            // }
            foreach (var keyValue in config.GetSection("OpenTelemetry:Instrumentation:AspNetCore:HttpResponseFields").GetChildren())
            {
                try
                {
                    httpResponseExpressionEvaluators.Add(keyValue.Key, ExpressionEvaluatorCompiler<HttpResponse>.Compile(keyValue.Value) ?? throw new Exception("ExpressionEvaluatorCompiler<HttpResponse>.Compile returned null"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"OpenTelemetry.cs [OpenTelemetryInstrumentation constructor] - failed to compile expression '{keyValue.Value}' for attribute '{keyValue.Key}': {ex.GetFullMessage()}");
                    throw;
                }
            }
            // OpenTelemetry:Instrumentation:HttpClient:HttpRequestMessageFields consists of a map of attribute name to C# expression.
            // The expression is evaluated against the HttpRequestMessage object and the result is set as the attribute value.
            // For example, to set the attribute "userId" to the value of the User.Identity.Name property, add the following to appsettings.json:
            // "OpenTelemetry:Instrumentation:HttpClient:HttpRequestMessageFields": {
            //     "userId": "HttpContext.User?.Identity?.Name"
            // }
            foreach (var keyValue in config.GetSection("OpenTelemetry:Instrumentation:HttpClient:HttpRequestMessageFields").GetChildren())
            {
                try
                {
                    httpRequestMessageExpressionEvaluators.Add(keyValue.Key, ExpressionEvaluatorCompiler<HttpRequestMessage>.Compile(keyValue.Value) ?? throw new Exception("ExpressionEvaluatorCompiler<HttpRequestMessage>.Compile returned null"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"OpenTelemetry.cs [OpenTelemetryInstrumentation constructor] - failed to compile expression '{keyValue.Value}' for attribute '{keyValue.Key}': {ex.GetFullMessage()}");
                    throw;
                }
            }
            // OpenTelemetry:Instrumentation:HttpClient:HttpResponseMessageFields consists of a map of attribute name to C# expression.
            // The expression is evaluated against the HttpResponseMessage object and the result is set as the attribute value.
            // For example, to set the attribute "statusCode" to the value of the StatusCode property, add the following to appsettings.json:
            // "OpenTelemetry:Instrumentation:HttpClient:HttpResponseMessageFields": {
            //     "statusCode": "StatusCode"
            // }
            foreach (var keyValue in config.GetSection("OpenTelemetry:Instrumentation:HttpClient:HttpResponseMessageFields").GetChildren())
            {
                try
                {
                    httpResponseMessageExpressionEvaluators.Add(keyValue.Key, ExpressionEvaluatorCompiler<HttpResponseMessage>.Compile(keyValue.Value) ?? throw new Exception("ExpressionEvaluatorCompiler<HttpResponseMessage>.Compile returned null"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"OpenTelemetry.cs [OpenTelemetryInstrumentation constructor] - failed to compile expression '{keyValue.Value}' for attribute '{keyValue.Key}': {ex.GetFullMessage()}");
                    throw;
                }
            }

            OpenTelemetry.Sdk.SetDefaultTextMapPropagator(new CompositeTextMapPropagator(new TextMapPropagator[] { new TraceContextPropagator(), new BaggagePropagator() }));
            var commonConfig = config.GetSection("Common");
            services.AddOpenTelemetry()
                .WithTracing(builder => builder
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.EnrichWithHttpRequest = (activity, request) =>
                        {
                            foreach (var evaluator in httpRequestExpressionEvaluators)
                            {
                                try
                                {
                                    var value = evaluator.Value(request);
                                    activity.SetTag(evaluator.Key, value);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"OpenTelemetry.cs [OpenTelemetryInstrumentation.Configure] - failed to evaluate expression '{evaluator.Key}': {ex.GetFullMessage()}");
                                }
                            }
                        };
                        options.EnrichWithHttpResponse = (activity, response) =>
                        {
                            foreach (var evaluator in httpResponseExpressionEvaluators)
                            {
                                try
                                {
                                    activity.SetTag(evaluator.Key, evaluator.Value(response));
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"OpenTelemetry.cs [OpenTelemetryInstrumentation.Configure] - failed to evaluate expression '{evaluator.Key}': {ex.GetFullMessage()}");
                                }
                            }
                        };
                        options.EnrichWithException = (activity, exception) =>
                        {
                            if (config.GetValue("OpenTelemetry:Instrumentation:AspNetCore:Exception:IncludeType", false))
                            {
                                activity.SetTag("Type", exception.GetType());
                            }
                            if (config.GetValue("OpenTelemetry:Instrumentation:AspNetCore:Exception:IncludeMessage", false))
                            {
                                activity.SetTag("Message", exception.GetFullMessage());
                            }
                            if (config.GetValue("OpenTelemetry:Instrumentation:AspNetCore:Exception:IncludeStackTrace", false))
                            {
                                activity.SetTag("StackTrace", exception.StackTrace);
                            }
                        };
                        options.RecordException = config.GetValue("OpenTelemetry:Instrumentation:AspNetCore:Exception:Record", false);
                    })
                    .AddHttpClientInstrumentation(options =>
                    {
                        options.EnrichWithHttpRequestMessage = (activity, request) =>
                        {
                            foreach (var evaluator in httpRequestMessageExpressionEvaluators)
                            {
                                try
                                {
                                    activity.SetTag(evaluator.Key, evaluator.Value(request));
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"OpenTelemetry.cs [OpenTelemetryInstrumentation.Configure] - failed to evaluate expression '{evaluator.Key}': {ex.GetFullMessage()}");
                                }
                            }
                        };
                        options.EnrichWithHttpResponseMessage = (activity, response) =>
                        {
                            foreach (var evaluator in httpResponseMessageExpressionEvaluators)
                            {
                                try
                                {
                                    activity.SetTag(evaluator.Key, evaluator.Value(response));
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"OpenTelemetry.cs [OpenTelemetryInstrumentation.Configure] - failed to evaluate expression '{evaluator.Key}': {ex.GetFullMessage()}");
                                }
                            }
                        };
                        options.EnrichWithException = (activity, exception) =>
                        {
                            if (config.GetValue("OpenTelemetry:Instrumentation:HttpClient:Exception:IncludeType", false))
                            {
                                activity.SetTag("Type", exception.GetType());
                            }
                            if (config.GetValue("OpenTelemetry:Instrumentation:HttpClient:Exception:IncludeMessage", false))
                            {
                                activity.SetTag("Message", exception.GetFullMessage());
                            }
                            if (config.GetValue("OpenTelemetry:Instrumentation:HttpClient:Exception:IncludeStackTrace", false))
                            {
                                activity.SetTag("StackTrace", exception.StackTrace);
                            }
                        };
                        options.RecordException = config.GetValue("OpenTelemetry:Instrumentation:HttpClient:Exception:Record", false);
                    })
                    .ConfigureResource(r => r.AddService("ze-alert-service"))
                    .ConfigureResource(r => r.AddAttributes(new[] { new KeyValuePair<string, object>("component", "backend") }))
                    // Add all key-value pairs from commonConfig Attributes section
                    .ConfigureResource(r => r.AddAttributes(commonConfig.GetSection("Attributes").GetChildren().Select(c => new KeyValuePair<string, object>(c.Key, c.Value))))
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(config["OpenTelemetry:CollectorUrl"]);
                        var headers = config.GetSection("OpenTelemetry:Headers").GetChildren();
                        options.Headers = string.Join(";", headers.Select(h => $"{h.Key}={h.Value}"));
                    })
                );
        }
    }

    // ExpressionEvaluatorCompiles compiled the given Linq expression at runtime
    // and returns a Func<T, object> that can be used to evaluate the expression
    // on instances of type T.
    public static class ExpressionEvaluatorCompiler<T>
    {
        public static Func<T, object>? Compile(string expression)
        {
            LambdaExpression exp = DynamicExpressionParser.ParseLambda(typeof(T), typeof(object), expression);
            return exp.Compile() as Func<T, object>;
        }
    }

    public class Configuration : Dictionary<string, object>
    {
        public Configuration(IConfigurationSection config)
        : base(LoadSection(config) as Dictionary<string, object> ?? new Dictionary<string, object>())
        {
        }

        private static object LoadSection(IConfigurationSection section)
        {
            var children = section.GetChildren();
            if (children.Any())
            {
                // Is it a dictionary or an array? We look at the type of
                // the key to see
                var firstKey = children.First().Key;
                if (int.TryParse(firstKey, out var index))
                {
                    // It's an array
                    return children.Select(c => LoadSection(c)).ToArray();
                }
                else
                {
                    // It's a dictionary
                    return children.ToDictionary(c => c.Key, c => LoadSection(c));
                }
            }
            else
            {
                if (bool.TryParse(section.Value, out var boolValue))
                {
                    return boolValue;
                }
                else if (int.TryParse(section.Value, out var intValue))
                {
                    return intValue;
                }
                else
                {
                    return section.Value;
                }
            }
        }

        public bool Enabled
        {
            get
            {
                return this.GetValueOrDefault("Enabled", false) as bool? ?? false;
            }
        }
    }
}
