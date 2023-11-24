using VGStandard.Data.Infrastructure.Repositories.Interfaces;
using VGStandard.Data.Infrastructure.Repositories.Postgres;
using ZeroEyes.Management.Application.Services;

namespace VGStandard.WebAPI.Extensinos
{
    public static class ServiceCollectionExteions
    {

        public static IServiceCollection AddServiceLayer(this IServiceCollection services)
        {
            services.AddTransient<ISystemService, SystemService>();
            services.AddTransient<IRegionService, RegionService>();
            services.AddTransient<IReleaseService, ReleaseService>();
            services.AddTransient<IRomService, RomService>();
            return services;
        }

        public static IServiceCollection AddDataAccessLayer(this IServiceCollection services)
        {
            services.AddTransient<ISystemRepository, SystemRepository>();
            services.AddTransient<IRegionRepository, RegionRepository>();
            services.AddTransient<IReleaseRepository, ReleaseRepository>();
            services.AddTransient<IRomRepository, RomRepository>();
            return services;
        }
    }
}
