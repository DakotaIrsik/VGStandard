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
public class RegionsController : BaseController
{
    private readonly IRegionService _service;
    public RegionsController(IMapper mapper,
                            IOptions<AppSettings> options,
                            ILogger<RegionsController> logger,
                            IRegionService service,
                            IHttpContextAccessor context) : base(context, mapper, options, logger)
    {
        _service = service;
    }

    /// <summary>
    /// Returns a pageable list of Regions that meet the search criteria
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RegionViewModel>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RegionViewModel>>> GetAsync([FromQuery] ApiRequest request)
    {
        var result = await _service.ReadAsync(request);
        var response = _mapper.Map<List<RegionViewModel>>(result.Data);
        return StandardResponse<RegionViewModel>(request, result.Total, response.FieldSelect(request));
    }

    ///// <summary>
    ///// Returns a pageable list of Regions that meet the search criteria
    ///// </summary>
    ///// <returns></returns>
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RegionViewModel>))]
    //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //[HttpGet("Search")]
    //public async Task<ActionResult<IEnumerable<RegionViewModel>>> SearchAsync([FromQuery] Lista<SearchParameter> requests)
    //{
    //    var result = await _service.SearchAsync(new ApiRequest(), request);az
    //    var response = _mapper.Map<List<RegionViewModel>>(result.Data);
    //    return StandardResponse<RegionViewModel>(request, result.Total, response.FieldSelect(request.FieldSelect());
    //}

    /// <summary>
    /// Returns single Region
    /// </summary>
    /// <param name="id">The Id of the Region</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegionViewModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("{id}")]
    public async Task<ActionResult<RegionViewModel>> GetAsync(int id)
    {
        var result = await _service.ReadAsync(new ApiRequest() { });
        var response = _mapper.Map<RegionViewModel>(result);
        return Ok(response);
    }


    /// <summary>
    /// Adds a single Region
    /// </summary>
    /// <param name="id">The Id of the Region</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RegionViewModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost]
    public async Task<ActionResult<RegionViewModel>> PostAsync([FromBody] CreateRegionViewModel model)
    {
        var request = _mapper.Map<RegionDTO>(model);
        var result = _service.Add(request);
        var response = _mapper.Map<RegionViewModel>(result);
        return Created(GetResourceLink(request.Id.ToString()), response);
    }


    /// <summary>
    /// Adds a array of Regions
    /// </summary>
    /// <param name="fields">The Fields to be sent back through websocket and api</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RegionViewModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost("Batch")]
    public async Task<ActionResult<RegionViewModel>> PostAsync([FromBody] List<CreateRegionViewModel> models, [FromQuery] string? fields)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (models.Count > Constants.MaxBatchSize)
        {
            return BatchSizeExceeded();
        }

        var response = _service.Add(_mapper.Map<List<RegionDTO>>(models));
        return StandardResponse<RegionViewModel>(new ApiRequest() { PageSize = models.Count, Fields = fields }, models.Count(), response.Select(r => r.FieldSelect(fields)));
    }

    /// <summary>
    /// Updates a single Region
    /// </summary>
    /// <param name="id">The Id of the Region</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(RegionViewModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPut]
    public async Task<ActionResult<RegionViewModel>> PutAsync([FromBody] UpdateRegionViewModel model)
    {
        var request = _mapper.Map<RegionDTO>(model);
        var result = _service.Update(request);

        var response = _mapper.Map<RegionViewModel>(result);
        return Accepted(GetResourceLink(model.Id.ToString()), response);
    }


    /// <summary>
    /// Remove an Region
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
    /// Remove an array of Regions by their Primary Key (Region.Id)
    /// </summary>
    /// <param name="id">The Ids of the Region</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpDelete("Batch")]
    public ActionResult BatchDelete(List<long> Ids)
    {
        _service.BatchDelete(Ids.Select(a => new RegionDTO() { Id = a }));
        return NoContent();
    }
}
