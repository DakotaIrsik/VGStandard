using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VGStandard.Application;
using VGStandard.Application.DTOs;
using VGStandard.Core.Metadata;
using VGStandard.Core.Metadata.Paging;
using VGStandard.Core.Settings;
using VGStandard.WebAPI.ViewModels;
using VGStandard.Application.Services;

namespace VGStandard.WebAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ReleasesController : BaseController
{
    private readonly IReleaseService _service;
    public ReleasesController(IMapper mapper,
                            IOptions<AppSettings> options,
                            ILogger<ReleasesController> logger,
                            IReleaseService service,
                            IHttpContextAccessor context) : base(context, mapper, options, logger)
    {
        _service = service;
    }

    /// <summary>
    /// Returns a pageable list of Releases that meet the search criteria
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ReleaseViewModel>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReleaseViewModel>>> GetAsync([FromQuery] ApiRequest request)
    {
        var result = await _service.ReadAsync(request);
        var response = _mapper.Map<List<ReleaseViewModel>>(result.Data);
        return StandardResponse<ReleaseViewModel>(request, result.Total, response.FieldSelect(request));
    }

    ///// <summary>
    ///// Returns a pageable list of Releases that meet the search criteria
    ///// </summary>
    ///// <returns></returns>
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ReleaseViewModel>))]
    //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //[HttpGet("Search")]
    //public async Task<ActionResult<IEnumerable<ReleaseViewModel>>> SearchAsync([FromQuery] Lista<SearchParameter> requests)
    //{
    //    var result = await _service.SearchAsync(new ApiRequest(), request);az
    //    var response = _mapper.Map<List<ReleaseViewModel>>(result.Data);
    //    return StandardResponse<ReleaseViewModel>(request, result.Total, response.FieldSelect(request.FieldSelect());
    //}

    /// <summary>
    /// Returns single Release
    /// </summary>
    /// <param name="id">The Id of the Release</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReleaseViewModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("{id}")]
    public async Task<ActionResult<ReleaseViewModel>> GetAsync(int id)
    {
        var result = await _service.ReadAsync(new ApiRequest() { });
        var response = _mapper.Map<ReleaseViewModel>(result);
        return Ok(response);
    }


    /// <summary>
    /// Adds a single Release
    /// </summary>
    /// <param name="id">The Id of the Release</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReleaseViewModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost]
    public async Task<ActionResult<ReleaseViewModel>> PostAsync([FromBody] CreateReleaseViewModel model)
    {
        var request = _mapper.Map<ReleaseDTO>(model);
        var result = _service.Add(request);
        var response = _mapper.Map<ReleaseViewModel>(result);
        return Created(GetResourceLink(request.Id.ToString()), response);
    }


    /// <summary>
    /// Adds a array of Releases
    /// </summary>
    /// <param name="fields">The Fields to be sent back through websocket and api</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReleaseViewModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost("Batch")]
    public async Task<ActionResult<ReleaseViewModel>> PostAsync([FromBody] List<CreateReleaseViewModel> models, [FromQuery] string? fields)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (models.Count > Constants.MaxBatchSize)
        {
            return BatchSizeExceeded();
        }

        var response = _service.Add(_mapper.Map<List<ReleaseDTO>>(models));
        return StandardResponse<ReleaseViewModel>(new ApiRequest() { PageSize = models.Count, Fields = fields }, models.Count(), response.Select(r => r.FieldSelect(fields)));
    }

    /// <summary>
    /// Updates a single Release
    /// </summary>
    /// <param name="id">The Id of the Release</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(ReleaseViewModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPut]
    public async Task<ActionResult<ReleaseViewModel>> PutAsync([FromBody] UpdateReleaseViewModel model)
    {
        var request = _mapper.Map<ReleaseDTO>(model);
        var result = _service.Update(request);

        var response = _mapper.Map<ReleaseViewModel>(result);
        return Accepted(GetResourceLink(model.Id.ToString()), response);
    }


    /// <summary>
    /// Remove an Release
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
    /// Remove an array of Releases by their Primary Key (Release.Id)
    /// </summary>
    /// <param name="id">The Ids of the Release</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpDelete("Batch")]
    public ActionResult BatchDelete(List<long> Ids)
    {
        _service.BatchDelete(Ids.Select(a => new ReleaseDTO() { Id = a }));
        return NoContent();
    }
}
