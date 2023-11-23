using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NpgsqlTypes;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;
using System.Net;
using System.Security.Authentication;
using VGStandard.Common.Web.Middleware;
using VGStandard.Core.Settings;

namespace VGStandard.Common.Web.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static Logger GetSeriloger(AppSettings settings)
    {

        var minLogLevel = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), settings.LogLevel);
        //        IDictionary<string, ColumnWriterBase> columnOptions = new Dictionary<string, ColumnWriterBase>
        //        {
        //            { "Message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
        //            { "MessageTemplate", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
        //            { "Level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
        //            { "RaiseDate", new TimestampColumnWriter(NpgsqlDbType.TimestampTz) },
        //            { "Exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
        //            { "Id", new ColumnWr IdAutoIncrementColumnWriter() } //TODO
        //        };


        return new LoggerConfiguration()
            //.WriteTo.PostgreSQL(settings.ConnectionStrings.Postgres,
            //                    typeof(ApiLog).Name,
            //                    columnOptions,
            //                    useCopy: false,
            //                   restrictedToMinimumLevel: minLogLevel,
            //                   period: new TimeSpan(0, 0, 2),
            //                   formatProvider: null)
            .WriteTo.Console(restrictedToMinimumLevel: minLogLevel)
            //.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(settings.ConnectionStrings.ElasticSearch))
            //{
            //    ModifyConnectionSettings = x => x.BasicAuthentication("elastic", settings.ElasticSearchApiKey),
            //    AutoRegisterTemplate = true,
            //    IndexFormat = settings.Environment.ToLower() + "-api-logs" + "-{0:yyyy.MM}",

            //    FailureCallback = e => Console.WriteLine("Error sending error through Serilog's Elasticsearch sink."),
            //    EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
            //                           EmitEventFailureHandling.WriteToFailureSink |
            //                           EmitEventFailureHandling.RaiseCallback |
            //                           EmitEventFailureHandling.ThrowException,
            //    MinimumLogEventLevel = minLogLevel
            //})
            .CreateLogger();
    }

    public static WebApplicationBuilder UseWebServer(this WebApplicationBuilder builder, AppSettings settings)
    {

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ConfigureHttpsDefaults(opt =>
                opt.ClientCertificateMode =
                    ClientCertificateMode.AllowCertificate);
        }).UseKestrel(options =>
        {
            options.ConfigureHttpsDefaults(httpsOptions =>
            {
                httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
            });

            options.Limits.MaxConcurrentConnections = settings.MaxConcurrentConnections;
            options.Limits.MaxConcurrentUpgradedConnections = settings.MaxConcurrentUpgradedConnections; //These are wss for SigR that don't hit a Db.

            options.Listen(IPAddress.Any, 80);
            options.Listen(IPAddress.Any, 443,
                listenOptions => { listenOptions.UseHttps("ze.pfx", @"vRrVg72gaYNdGF9rgQSJ"); });
        });

        return builder;
    }

    public static IApplicationBuilder UseSwaggerWithVersioning(this IApplicationBuilder app, IWebHostEnvironment environment)
    {
        IServiceProvider services = app.ApplicationServices;
        var provider = services.GetRequiredService<IApiVersionDescriptionProvider>();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
            }
        });

        return app;
    }

    public static bool IsDebug(this IWebHostEnvironment env)
    {
        return env.EnvironmentName.ToLower().Contains("debug");
    }

    public static bool IsProduction(this IWebHostEnvironment env)
    {
        return env.EnvironmentName.ToLower().Contains("prod");
    }

    public static bool IsAContainer(this IWebHostEnvironment env)
    {
        return env.EnvironmentName.ToLower().Contains("docker");
    }

    public static IApplicationBuilder RespondToRobots(this IApplicationBuilder app)
    {
        return app.UseWhen(
            context => context.Request.IsRobotsTxt(),
            appBuilder =>
            {
                appBuilder.UseMiddleware<RobotHandlingMiddleware>();
            });
    }

    public static IApplicationBuilder UseExceptionHandlers(this IApplicationBuilder app, IHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }
        return app;
    }

    public static IApplicationBuilder ProtectFolder(this IApplicationBuilder app, ProtectFolderOptions options)
    {
        if (options == null)
        {
            options = new ProtectFolderOptions
            {
                Path = "/Secret",
                PolicyName = "Authenticated"
            };
        }

        return app.UseMiddleware<ProtectFolder>(options);
    }
}
