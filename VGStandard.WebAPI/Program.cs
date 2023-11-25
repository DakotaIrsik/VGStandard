using Microsoft.EntityFrameworkCore;
using Serilog;
using VGStandard.Common.Web.ActionFilters;
using VGStandard.Common.Web.Extensions;
using VGStandard.Common.Web.Middleware;
using VGStandard.Core.Settings;
using VGStandard.Data.Infrastructure.Contexts;
using VGStandard.WebAPI.Extensinos;

var builder = WebApplication.CreateBuilder(args);
var settings = builder.Configuration.Get<AppSettings>();

#region Configure WebServer and services

builder.Services.AddSignalR();
builder.Services.AddCors(opt => opt.AddPolicy("CorsPolicy", c =>
{
    c.AllowAnyOrigin()
     .AllowAnyHeader()
     .AllowAnyMethod();
}));
builder.Services.AddCacheService(settings);
builder.Services.AddSwagger(builder.Environment, "VGStandard");
builder.Services.AddAppSettingsIoC(builder.Configuration);
builder.Services.AddDbContext<VideoGameContext>(opts => opts.UseNpgsql(settings.ConnectionStrings.Postgres));
builder.Services.AddServiceLayer();
builder.Services.AddDataAccessLayer();
builder.Services.AddScoped(sp => new PoorManClientCredentialFilter(settings.PoorManClientCredential, builder.Environment));
builder.Services.AddRouting();
builder.Services.AddAutoMapper();
builder.Services.AddTrackability();
builder.Services.AddHttpContextAccessor();
builder.Services.AddPresentationLayer(builder.Environment);
builder.Services.AddControllers();
builder.Services.AddResponseCaching();
builder.AddRestIntegrations(settings);
builder.UseWebServer(settings);
builder.Host.UseSerilog(WebApplicationBuilderExtensions.GetSeriloger(settings));

#endregion

#region Build and Run
var app = builder.Build();
app.UseResponseCaching();

app.UseSwaggerWithVersioning(app.Environment);
app.UseRouting();
app.UseAuthorization();
app.UseCors("CorsPolicy");
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseEndpoints(endpoints => endpoints.MapControllers());
app.RespondToRobots();
app.UseExceptionHandlers(builder.Environment);
app.Run();
#endregion