using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using VGStandard.Core.Settings;

namespace VGStandard.Common.Web.Options;

public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider provider;
    private readonly AppSettings _settings;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IOptions<AppSettings> options)
    {
        this.provider = provider;
        _settings = options.Value;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateVersionInfo(description));
        }
    }

    public void Configure(string name, SwaggerGenOptions options)
    {
        Configure(options);
    }

    private OpenApiInfo CreateVersionInfo(ApiVersionDescription description)
    {
        var info = new OpenApiInfo()
        {
            Title = $"{_settings.Application} WebAPI - ({_settings.Environment})",
            Version = description.ApiVersion.ToString(),
            Description = $"{System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}"
        };

        if (description.IsDeprecated)
        {
            info.Description += " This API version has been deprecated.";
        }

        return info;
    }
}
