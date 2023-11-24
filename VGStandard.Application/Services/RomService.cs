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

public interface IRomService
{
    Task<StandardResponse<RomDTO>> ReadAsync(ApiRequest request);
    public Task<RomDTO> ReadAsync(long Id);
    public Task<RomDTO> Add(RomDTO model);
    public List<RomDTO> Add(List<RomDTO> models);
    public RomDTO Update(RomDTO model);
    public void Delete(long id);
    public void BatchDelete(IEnumerable<RomDTO> alerts);
}

public class RomService : IRomService
{
    private IRomRepository _respository { get; set; }
    private IConfigurationProvider _configurationProvider { get; set; }
    private IMapper _mapper { get; set; }

    public RomService(IRomRepository repository,
                        IMapper mapper,
                        IConfigurationProvider configurationProvider,
                        ILogger<RomService> logger,
                        IOptions<AppSettings> options)
    {

        _respository = repository;
        _configurationProvider = configurationProvider;
        _mapper = mapper;
    }

    public async Task<StandardResponse<RomDTO>> ReadAsync(ApiRequest request)
    {
        var response = await _respository.Read()
                                        .SortBy(request)
                                        .Paginate(request)
                                        .ProjectTo<RomDTO>(_configurationProvider)
                                        .ToListAsync();

        return new StandardResponse<RomDTO>() { Data = response, Total = response.Count() };
    }


    public async Task<RomDTO> ReadAsync(long Id)
    {
        var response = await _respository.Read().SingleOrDefaultAsync(a => a.Id == Id);
        return _mapper.Map<RomDTO>(response);
    }

    public async Task<RomDTO> Add(RomDTO model)
    {
        var request = _mapper.Map<Rom>(model);
        _respository.Add(request);
        return _mapper.Map<RomDTO>(_respository.Read().SingleOrDefault(r => r.Id == request.Id));
    }

    public List<RomDTO> Add(List<RomDTO> models)
    {
        var result = _respository.Add(_mapper.Map<List<Rom>>(models));
        return _mapper.Map<List<RomDTO>>(result);
    }

    public RomDTO Update(RomDTO model)
    {
        var request = _mapper.Map<Rom>(model);
        _respository.Update(request);
        return _mapper.Map<RomDTO>(_respository.Read().SingleOrDefault(r => r.Id == request.Id));
    }

    public void Delete(long id)
    {
        _respository.Delete(id);
    }

    public void BatchDelete(IEnumerable<RomDTO> alerts)
    {
        var request = _mapper.Map<List<Rom>>(alerts);
        _respository.BatchDelete(request);
    }
}