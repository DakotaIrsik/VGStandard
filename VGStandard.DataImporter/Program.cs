using AutoMapper;
using CommandLine;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Reflection;
using VGStandard.Common.Web.Extensions;
using VGStandard.Common.Web.Services;
using VGStandard.Core.Metadata;
using VGStandard.Core.Settings;
using VGStandard.Data.Contexts;
using VGStandard.DataImporter.Models;

namespace VGStandard.Data.Importer;

class Program
{
    static async Task Main(string[] args)
    {
        Parser.Default.ParseArguments<CommandLineOptions>(args)
            .WithParsed(async options =>
            {
                var host = CreateHostBuilder(args, options).Build();
                var app = host.Services.GetRequiredService<Importer>();
                await app.Run(options);
            });
    }

    static IHostBuilder CreateHostBuilder(string[] args, CommandLineOptions options) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddCommandLine(args);
            })
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;
                var settings = services.AddAppSettingsIoC(configuration);

                string elasticSearchUrl = options.ElasticsearchUrl ?? settings.ConnectionStrings.ElasticSearch;
                string apiKey = options.ElasticsearchApiKey ?? settings.ElasticSearchApiKey;
                string postgresConnectionString = options.PostgresConnectionString ?? settings.ConnectionStrings.Postgres;

                services.AddDbContext<VideoGameContext>(opts =>
                    opts.UseNpgsql(postgresConnectionString));

                services.AddTransient<IElasticSearchService>(provider =>
                    new ElasticSearchService(provider.GetRequiredService<IOptions<AppSettings>>(),
                                             provider.GetRequiredService<IMapper>(),
                                             elasticSearchUrl, apiKey));
                services.AddAutoMapper(Assembly.GetExecutingAssembly());
                services.AddCacheService(settings);
                services.AddSingleton<Importer>();
            });
}

public class Importer
{
    private readonly IElasticSearchService _elasticSearchService;
    private readonly VideoGameContext _context;
    private readonly ILogger<Importer> _logger;
    private readonly AppSettings _settings;

    public Importer(IElasticSearchService elasticSearchService,
                 VideoGameContext context,
                 ILogger<Importer> logger,
                 IOptions<AppSettings> options)
    {
        _elasticSearchService = elasticSearchService;
        _context = context;
        _logger = logger;
        _settings = options.Value;
    }

    public async Task Run(CommandLineOptions options)
    {
        if (options.RecreatePostgresTables || _settings.RecreatePostgresTables)
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        bool useBulkPostgres = options.BulkPostgres || _settings.BulkPostgres;
        bool useBulkElasticsearch = options.BulkElasticsearch || _settings.BulkElasticSearch;
        bool skipPostgres = options.SkipPostgres || _settings.SkipPostgres;
        bool skipElasticSearch = options.SkipElasticsearch || _settings.SkipElasticsearch;

        if (!skipPostgres)
        {
            if (useBulkPostgres)
            {
                BulkPopulatePostgres();
            }
            else
            {
                await PopulatePostgres();
            }
        }

        if (!skipElasticSearch)
        {
            _elasticSearchService.DeleteIndex("systems");
            _elasticSearchService.DeleteIndex("regions");
            _elasticSearchService.DeleteIndex("roms");
            _elasticSearchService.DeleteIndex("releases");

            _elasticSearchService.CreateIndex("systems", File.ReadAllText("Models\\ElasticSearch\\Mappings\\ElasticSystems.json"));
            _elasticSearchService.CreateIndex("regions", File.ReadAllText("Models\\ElasticSearch\\Mappings\\ElasticRegions.json"));
            _elasticSearchService.CreateIndex("roms", File.ReadAllText("Models\\ElasticSearch\\Mappings\\ElasticRoms.json"));
            _elasticSearchService.CreateIndex("releases", File.ReadAllText("Models\\ElasticSearch\\Mappings\\ElasticReleases.json"));


            if (useBulkElasticsearch)
            {
                await BulkPopulateElastic();
            }
            else
            {
                PopulateElastic();
            }
        }
    }



    private void BulkPopulatePostgres()
    {
         BulkPopulateTable<Region>("regions.json");
         BulkPopulateTable<GameSystem>("systems.json");
         BulkPopulateTable<Rom>("roms.json");
         BulkPopulateTable<Release>("releases.json");
    }

    private async Task PopulatePostgres()
    {
        await PopulateTable<Region>("regions.json");
        await PopulateTable<GameSystem>("systems.json");
        await PopulateTable<Rom>("roms.json");
        await PopulateTable<Release>("releases.json");
    }

    private async Task BulkPopulateElastic()
    {
        BulkPopulateElastic<Region>("regions.json", "regions");
        BulkPopulateElastic<GameSystem>("systems.json", "systems");
        BulkPopulateElastic<Rom>("roms.json", "roms");
        BulkPopulateElastic<Release>("releases.json", "releases");
    }

    private async Task PopulateElastic()
    {
        PopulateIndex<Region>("regions.json", "regions");
        PopulateIndex<GameSystem>("systems.json", "systems");
        PopulateIndex<Rom>("roms.json", "roms");
        PopulateIndex<Release>("releases.json", "releases");
    }

    private async Task PopulateTable<T>(string filePath) where T : Trackable, new()
    {
        try
        {
            var entities = JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(filePath));
            foreach (var entity in entities)
            {
                _context.Add(entity);
                _context.SaveChanges();
                _logger.LogInformation($"Successfully populated {entities.Count()} records into Postgres for table {typeof(T).Name}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while populating table for {typeof(T).Name}.");
        }
    }

    private void PopulateIndex<T>(string filePath, string indexName) where T : Trackable, new()
    {
        try
        {
            var entities = JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(filePath));
            foreach (var entity in entities)
            {
                var result = _elasticSearchService.Index(entity, indexName);
                _logger.LogInformation($"Successfully indexed {entities.Count()} records into ElasticSearch for index {typeof(T).Name}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while indexing documents in {indexName}.");
        }
    }

    private void BulkPopulateTable<T>(string filePath) where T : class, new()
    {
        try
        {
            var entities = JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(filePath));
            if (entities?.Count > 0)
            {
                _context.BulkInsert(entities);
                _logger.LogInformation($"Successfully bulk populated {entities.Count()} records into Postgres for table {typeof(T).Name}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred during bulk population for {typeof(T).Name}.");
        }
    }

    private void BulkPopulateElastic<T>(string filePath, string indexName) where T : class
    {
        try
        {
            var entities = JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(filePath));
            var documents = _elasticSearchService.IndexMany(entities, indexName);
            _logger.LogInformation($"Successfully bulk indexed {entities.Count()} records into ElasticSearch for index {typeof(T).Name}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while indexing documents in {indexName}.");
        }
    }
}

public class IndexConfiguration
{
    public Analysis Analysis { get; set; }
    public Mappings Mappings { get; set; }
}

public class Analysis
{
    public Dictionary<string, Analyzer> Analyzer { get; set; }
}

public class Analyzer
{
    public string Tokenizer { get; set; }
    public List<string> Filter { get; set; }
}

public class Mappings
{
    // Define your mappings properties here
}
