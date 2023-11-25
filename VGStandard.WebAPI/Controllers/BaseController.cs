using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Dynamic;
using System.Text.Json;
using VGStandard.Application;
using VGStandard.Common.Web.Responses;
using VGStandard.Core.Metadata;
using VGStandard.Core.Settings;

namespace VGStandard.WebAPI.Controllers;

public partial class BaseController : ControllerBase
{
    protected readonly IHttpContextAccessor _context;
    protected readonly ILogger<BaseController> _logger;
    protected readonly IMapper _mapper;
    protected AppSettings _settings;

    public BaseController(IHttpContextAccessor context,
                        IMapper mapper,
                        IOptions<AppSettings> options,
                        ILogger<BaseController> logger)
    {
        _context = context;
        _mapper = mapper;
        _settings = options.Value;
        _logger = logger;

    }
    protected BadRequestObjectResult BatchSizeExceeded() => new BadRequestObjectResult(new { Error = "Largest supported batch size is " + Constants.MaxBatchSize });

    protected ActionResult StandardResponse<T>(ApiRequest request, long total, IEnumerable<ExpandoObject> response)
    {
        var fields = string.IsNullOrWhiteSpace(request?.Fields) ?
                           string.Join(",", typeof(T).GetProperties().Select(p => p.Name.ToLower())) :
                           request.Fields.ToLower();

        return Ok(new
        {
            Total = total,
            From = request?.CurrentPage,
            Size = request?.PageSize,
            Sort = request?.Sort,
            Fields = fields,
            Data = response
        });
    }

    protected string LowerCaseJsonSerializer(object json)
    {
        /// consolidate with startup.
        var options = new JsonOptions();
        options.JsonSerializerOptions.IgnoreReadOnlyFields = true;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = new LowerCaseNamingPolicy();

        var x = JsonSerializer.Serialize(json, options.JsonSerializerOptions);
        return x;
    }

    protected string GetResourceLink(string id)
    {
        var request = _context?.HttpContext?.Request;
        UriBuilder uriBuilder = new UriBuilder();
        if (request.Host.Port.HasValue && request.Host.Port != 80 && request.Host.Port != 443)
        {
            uriBuilder.Port = request.Host.Port.Value;
        }
        uriBuilder.Scheme = request.Scheme;
        uriBuilder.Host = request.Host.Host;
        uriBuilder.Path = request.Path.ToString();
        uriBuilder.Query = request.QueryString.ToString();
        return $"{uriBuilder.Uri}/{id}";
    }
}

