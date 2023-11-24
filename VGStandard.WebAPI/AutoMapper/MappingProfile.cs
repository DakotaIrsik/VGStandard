using VGStandard.Application.DTOs;
using VGStandard.DataImporter.Models;
using VGStandard.WebAPI.ViewModels;
using Profile = AutoMapper.Profile;

namespace ZeroEyes.Management.Services.BFF.AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        #region ViewModels to DTOs
        CreateMap<CreateRomViewModel, RomDTO>();
        CreateMap<UpdateRomViewModel, RomDTO>();
        CreateMap<CreateSystemViewModel, SystemDTO>();
        CreateMap<UpdateSystemViewModel, SystemDTO>();
        CreateMap<CreateRegionViewModel, RegionDTO>();
        CreateMap<UpdateRegionViewModel, RegionDTO>();
        CreateMap<CreateReleaseViewModel, ReleaseDTO>();
        CreateMap<UpdateReleaseViewModel, ReleaseDTO>();
        #endregion

        #region DTOs to ViewModels
        CreateMap<RegionDTO, RegionViewModel>();
        CreateMap<RomDTO, RomViewModel>();
        CreateMap<SystemDTO, SystemViewModel>();
        CreateMap<ReleaseDTO, ReleaseViewModel>();
        #endregion

        #region DTOs to Models
        CreateMap<RegionDTO, Region>();
        CreateMap<RomDTO, Rom>();
        CreateMap<SystemDTO, GameSystem>();
        CreateMap<ReleaseDTO, Release>();
        #endregion

        #region Models to DTOs
        CreateMap<Region, RegionDTO>();
        CreateMap<Rom, RomDTO>();
        CreateMap<GameSystem, SystemDTO>();
        CreateMap<Release, ReleaseDTO>();
        #endregion
    }
}
