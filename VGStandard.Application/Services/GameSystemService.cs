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

namespace ZeroEyes.Management.Application.Services;

public interface ISystemService
{
    Task<StandardResponse<SystemDTO>> ReadAsync(ApiRequest request);
    public Task<SystemDTO> ReadAsync(long Id);
    public Task<SystemDTO> Add(SystemDTO model);
    public List<SystemDTO> Add(List<SystemDTO> models);
    public SystemDTO Update(SystemDTO model);
    public void Delete(long id);
    public void BatchDelete(IEnumerable<SystemDTO> alerts);
}

public class SystemService : ISystemService
{
    private ISystemRepository _respository { get; set; }
    private IConfigurationProvider _configurationProvider { get; set; }
    private IMapper _mapper { get; set; }

    public SystemService(ISystemRepository repository,
                            IMapper mapper,
                            IConfigurationProvider configurationProvider,
                            ILogger<SystemService> logger,
                            IOptions<AppSettings> options)
    {

        _respository = repository;
        _configurationProvider = configurationProvider;
        _mapper = mapper;
    }

    public async Task<StandardResponse<SystemDTO>> ReadAsync(ApiRequest request)
    {
        var response = await _respository.Read()
                                        .SortBy(request)
                                        .Paginate(request)
                                        .ProjectTo<SystemDTO>(_configurationProvider)
                                        .ToListAsync();

        return new StandardResponse<SystemDTO>() { Data = response, Total = response.Count() };
    }


    public async Task<SystemDTO> ReadAsync(long Id)
    {
        var response = await _respository.Read().SingleOrDefaultAsync(a => a.Id == Id);
        return _mapper.Map<SystemDTO>(response);
    }

    public async Task<SystemDTO> Add(SystemDTO model)
    {
        var request = _mapper.Map<GameSystem>(model);
        _respository.Add(request);
        return _mapper.Map<SystemDTO>(_respository.Read().SingleOrDefault(r => r.Id == request.Id));
    }

    public List<SystemDTO> Add(List<SystemDTO> models)
    {
        var result = _respository.Add(_mapper.Map<List<GameSystem>>(models));
        return _mapper.Map<List<SystemDTO>>(result);
    }

    public SystemDTO Update(SystemDTO model)
    {
        var request = _mapper.Map<GameSystem>(model);
        _respository.Update(request);
        return _mapper.Map<SystemDTO>(_respository.Read().SingleOrDefault(r => r.Id == request.Id));
    }

    public void Delete(long id)
    {
        _respository.Delete(id);
    }

    public void BatchDelete(IEnumerable<SystemDTO> alerts)
    {
        var request = _mapper.Map<List<GameSystem>>(alerts);
        _respository.BatchDelete(request);
    }
}