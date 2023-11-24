using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VGStandard.Application.DTOs;
using VGStandard.Core.Metadata;
using VGStandard.Core.Metadata.Paging;
using VGStandard.Core.Settings;
using VGStandard.Data.Infrastructure.Repositories.Interfaces;
using VGStandard.DataImporter.Models;

namespace VGStandard.Application.Services;

public interface IRegionService
{
    Task<StandardResponse<RegionDTO>> ReadAsync(ApiRequest request);
    public Task<RegionDTO> ReadAsync(long Id);
    public Task<RegionDTO> Add(RegionDTO model);
    public List<RegionDTO> Add(List<RegionDTO> models);
    public RegionDTO Update(RegionDTO model);
    public void Delete(long id);
    public void BatchDelete(IEnumerable<RegionDTO> alerts);
}

public class RegionService : IRegionService
{
    private IRegionRepository _respository { get; set; }
    private IConfigurationProvider _configurationProvider { get; set; }
    private IMapper _mapper { get; set; }

    public RegionService(IRegionRepository repository,
                        IMapper mapper,
                        IConfigurationProvider configurationProvider,
                        ILogger<RegionService> logger,
                        IOptions<AppSettings> options)
    {

        _respository = repository;
        _configurationProvider = configurationProvider;
        _mapper = mapper;
    }

    public async Task<StandardResponse<RegionDTO>> ReadAsync(ApiRequest request)
    {
        var response = await _respository.Read()
                                        .SortBy(request)
                                        .Paginate(request)
                                        .ProjectTo<RegionDTO>(_configurationProvider)
                                        .ToListAsync();

        return new StandardResponse<RegionDTO>() { Data = response, Total = response.Count() };
    }


    public async Task<RegionDTO> ReadAsync(long Id)
    {
        var response = await _respository.Read().SingleOrDefaultAsync(a => a.Id == Id);
        return _mapper.Map<RegionDTO>(response);
    }

    public async Task<RegionDTO> Add(RegionDTO model)
    {
        var request = _mapper.Map<Region>(model);
        _respository.Add(request);
        return _mapper.Map<RegionDTO>(_respository.Read().SingleOrDefault(r => r.Id == request.Id));
    }

    public List<RegionDTO> Add(List<RegionDTO> models)
    {
        var result = _respository.Add(_mapper.Map<List<Region>>(models));
        return _mapper.Map<List<RegionDTO>>(result);
    }

    public RegionDTO Update(RegionDTO model)
    {
        var request = _mapper.Map<Region>(model);
        _respository.Update(request);
        return _mapper.Map<RegionDTO>(_respository.Read().SingleOrDefault(r => r.Id == request.Id));
    }

    public void Delete(long id)
    {
        _respository.Delete(id);
    }

    public void BatchDelete(IEnumerable<RegionDTO> alerts)
    {
        var request = _mapper.Map<List<Region>>(alerts);
        _respository.BatchDelete(request);
    }
}