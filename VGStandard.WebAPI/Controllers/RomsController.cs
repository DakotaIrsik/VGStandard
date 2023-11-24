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
public class RomsController : BaseController
{
    private readonly IRomService _service;
    public RomsController(IMapper mapper,
                            IOptions<AppSettings> options,
                            ILogger<RomsController> logger,
                            IRomService service,
                            IHttpContextAccessor context) : base(context, mapper, options, logger)
    {
        _service = service;
    }

    /// <summary>
    /// Returns a pageable list of Roms that meet the search criteria
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RomViewModel>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RomViewModel>>> GetAsync([FromQuery] ApiRequest request)
    {
        var result = await _service.ReadAsync(request);
        var response = _mapper.Map<List<RomViewModel>>(result.Data);
        return StandardResponse<RomViewModel>(request, result.Total, response.FieldSelect(request));
    }

    ///// <summary>
    ///// Returns a pageable list of Roms that meet the search criteria
    ///// </summary>
    ///// <returns></returns>
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RomViewModel>))]
    //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //[HttpGet("Search")]
    //public async Task<ActionResult<IEnumerable<RomViewModel>>> SearchAsync([FromQuery] Lista<SearchParameter> requests)
    //{
    //    var result = await _service.SearchAsync(new ApiRequest(), request);az
    //    var response = _mapper.Map<List<RomViewModel>>(result.Data);
    //    return StandardResponse<RomViewModel>(request, result.Total, response.FieldSelect(request.FieldSelect());
    //}

    /// <summary>
    /// Returns single Rom
    /// </summary>
    /// <param name="id">The Id of the Rom</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RomViewModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("{id}")]
    public async Task<ActionResult<RomViewModel>> GetAsync(int id)
    {
        var result = await _service.ReadAsync(new ApiRequest() { });
        var response = _mapper.Map<RomViewModel>(result);
        return Ok(response);
    }


    /// <summary>
    /// Adds a single Rom
    /// </summary>
    /// <param name="id">The Id of the Rom</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RomViewModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost]
    public async Task<ActionResult<RomViewModel>> PostAsync([FromBody] CreateRomViewModel model)
    {
        var request = _mapper.Map<RomDTO>(model);
        var result = _service.Add(request);
        var response = _mapper.Map<RomViewModel>(result);
        return Created(GetResourceLink(request.Id.ToString()), response);
    }


    /// <summary>
    /// Adds a array of Roms
    /// </summary>
    /// <param name="fields">The Fields to be sent back through websocket and api</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RomViewModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost("Batch")]
    public async Task<ActionResult<RomViewModel>> PostAsync([FromBody] List<CreateRomViewModel> models, [FromQuery] string? fields)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (models.Count > Constants.MaxBatchSize)
        {
            return BatchSizeExceeded();
        }

        var response = _service.Add(_mapper.Map<List<RomDTO>>(models));
        return StandardResponse<RomViewModel>(new ApiRequest() { PageSize = models.Count, Fields = fields }, models.Count(), response.Select(r => r.FieldSelect(fields)));
    }

    /// <summary>
    /// Updates a single Rom
    /// </summary>
    /// <param name="id">The Id of the Rom</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(RomViewModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPut]
    public async Task<ActionResult<RomViewModel>> PutAsync([FromBody] UpdateRomViewModel model)
    {
        var request = _mapper.Map<RomDTO>(model);
        var result = _service.Update(request);

        var response = _mapper.Map<RomViewModel>(result);
        return Accepted(GetResourceLink(model.Id.ToString()), response);
    }


    /// <summary>
    /// Remove an Rom
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
    /// Remove an array of Roms by their Primary Key (Rom.Id)
    /// </summary>
    /// <param name="id">The Ids of the Rom</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpDelete("Batch")]
    public ActionResult BatchDelete(List<long> Ids)
    {
        _service.BatchDelete(Ids.Select(a => new RomDTO() { Id = a }));
        return NoContent();
    }
}
