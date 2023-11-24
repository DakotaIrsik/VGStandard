using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VGStandard.Application;
using VGStandard.Application.DTOs;
using VGStandard.Core.Metadata;
using VGStandard.Core.Metadata.Paging;
using VGStandard.Core.Settings;
using VGStandard.WebAPI.ViewModels;
using ZeroEyes.Management.Application.Services;

namespace ZeroEyes.Management.Services.BFF.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class SystemsController : BaseController
{
    private readonly ISystemService _service;
    public SystemsController(IMapper mapper,
                            IOptions<AppSettings> options,
                            ILogger<SystemsController> logger,
                            ISystemService service,
                            IHttpContextAccessor context) : base(context, mapper, options, logger)
    {
        _service = service;
    }

    /// <summary>
    /// Returns a pageable list of Systems that meet the search criteria
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SystemViewModel>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SystemViewModel>>> GetAsync([FromQuery] ApiRequest request)
    {
        var result = await _service.ReadAsync(request);
        var response = _mapper.Map<List<SystemViewModel>>(result.Data);
        return StandardResponse<SystemViewModel>(request, result.Total, response.FieldSelect(request));
    }

    ///// <summary>
    ///// Returns a pageable list of Systems that meet the search criteria
    ///// </summary>
    ///// <returns></returns>
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SystemViewModel>))]
    //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //[HttpGet("Search")]
    //public async Task<ActionResult<IEnumerable<SystemViewModel>>> SearchAsync([FromQuery] Lista<SearchParameter> requests)
    //{
    //    var result = await _service.SearchAsync(new ApiRequest(), request);az
    //    var response = _mapper.Map<List<SystemViewModel>>(result.Data);
    //    return StandardResponse<SystemViewModel>(request, result.Total, response.FieldSelect(request.FieldSelect());
    //}

    /// <summary>
    /// Returns single System
    /// </summary>
    /// <param name="id">The Id of the System</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SystemViewModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("{id}")]
    public async Task<ActionResult<SystemViewModel>> GetAsync(int id)
    {
        var result = await _service.ReadAsync(new ApiRequest() { });
        var response = _mapper.Map<SystemViewModel>(result);
        return Ok(response);
    }


    /// <summary>
    /// Adds a single System
    /// </summary>
    /// <param name="id">The Id of the System</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SystemViewModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost]
    public async Task<ActionResult<SystemViewModel>> PostAsync([FromBody] CreateSystemViewModel model)
    {
        var request = _mapper.Map<SystemDTO>(model);
        var result = _service.Add(request);
        var response = _mapper.Map<SystemViewModel>(result);
        return Created(GetResourceLink(request.Id.ToString()), response);
    }


    /// <summary>
    /// Adds a array of Systems
    /// </summary>
    /// <param name="fields">The Fields to be sent back through websocket and api</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SystemViewModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost("Batch")]
    public async Task<ActionResult<SystemViewModel>> PostAsync([FromBody] List<CreateSystemViewModel> models, [FromQuery] string? fields)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (models.Count > Constants.MaxBatchSize)
        {
            return BatchSizeExceeded();
        }

        var response = _service.Add(_mapper.Map<List<SystemDTO>>(models));
        return StandardResponse<SystemViewModel>(new ApiRequest() { PageSize = models.Count, Fields = fields }, models.Count(), response.Select(r => r.FieldSelect(fields)));
    }

    /// <summary>
    /// Updates a single System
    /// </summary>
    /// <param name="id">The Id of the System</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(SystemViewModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPut]
    public async Task<ActionResult<SystemViewModel>> PutAsync([FromBody] UpdateSystemViewModel model)
    {
        var request = _mapper.Map<SystemDTO>(model);
        var result = _service.Update(request);

        var response = _mapper.Map<SystemViewModel>(result);
        return Accepted(GetResourceLink(model.Id.ToString()), response);
    }


    /// <summary>
    /// Remove an System
    /// </summary>
    /// <param name="id">The Id of the Alert</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpDelete]
    public ActionResult Delete(long id)
    {
        _service.Delete(id);
        return NoContent();
    }

    /// <summary>
    /// Remove an array of Systems by their Primary Key (System.Id)
    /// </summary>
    /// <param name="id">The Ids of the System</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpDelete("Batch")]
    public ActionResult BatchDelete(List<long> Ids)
    {
        _service.BatchDelete(Ids.Select(a => new SystemDTO() { Id = a }));
        return NoContent();
    }
}
