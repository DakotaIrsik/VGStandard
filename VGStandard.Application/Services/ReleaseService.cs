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

public interface IReleaseService
{
    Task<StandardResponse<ReleaseDTO>> ReadAsync(ApiRequest request);
    public Task<ReleaseDTO> ReadAsync(long Id);
    public Task<ReleaseDTO> Add(ReleaseDTO model);
    public List<ReleaseDTO> Add(List<ReleaseDTO> models);
    public ReleaseDTO Update(ReleaseDTO model);
    public void Delete(long id);
    public void BatchDelete(IEnumerable<ReleaseDTO> alerts);
}

public class ReleaseService : IReleaseService
{
    private IReleaseRepository _respository { get; set; }
    private IConfigurationProvider _configurationProvider { get; set; }
    private IMapper _mapper { get; set; }

    public ReleaseService(IReleaseRepository repository,
                        IMapper mapper,
                        IConfigurationProvider configurationProvider,
                        ILogger<ReleaseService> logger,
                        IOptions<AppSettings> options)
    {

        _respository = repository;
        _configurationProvider = configurationProvider;
        _mapper = mapper;
    }

    public async Task<StandardResponse<ReleaseDTO>> ReadAsync(ApiRequest request)
    {
        var response = await _respository.Read()
                                        .SortBy(request)
                                        .Paginate(request)
                                        .ProjectTo<ReleaseDTO>(_configurationProvider)
                                        .ToListAsync();

        return new StandardResponse<ReleaseDTO>() { Data = response, Total = response.Count() };
    }


    public async Task<ReleaseDTO> ReadAsync(long Id)
    {
        var response = await _respository.Read().SingleOrDefaultAsync(a => a.Id == Id);
        return _mapper.Map<ReleaseDTO>(response);
    }

    public async Task<ReleaseDTO> Add(ReleaseDTO model)
    {
        var request = _mapper.Map<Release>(model);
        _respository.Add(request);
        return _mapper.Map<ReleaseDTO>(_respository.Read().SingleOrDefault(r => r.Id == request.Id));
    }

    public List<ReleaseDTO> Add(List<ReleaseDTO> models)
    {
        var result = _respository.Add(_mapper.Map<List<Release>>(models));
        return _mapper.Map<List<ReleaseDTO>>(result);
    }

    public ReleaseDTO Update(ReleaseDTO model)
    {
        var request = _mapper.Map<Release>(model);
        _respository.Update(request);
        return _mapper.Map<ReleaseDTO>(_respository.Read().SingleOrDefault(r => r.Id == request.Id));
    }

    public void Delete(long id)
    {
        _respository.Delete(id);
    }

    public void BatchDelete(IEnumerable<ReleaseDTO> alerts)
    {
        var request = _mapper.Map<List<Release>>(alerts);
        _respository.BatchDelete(request);
    }
}