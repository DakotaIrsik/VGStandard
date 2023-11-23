using CommandLine;

public class CommandLineOptions
{

    [Option("fill-postgres", Required = false, HelpText = "PostgresConnectionString")]
    public string PostgresConnectionString { get; set; }

    [Option("fill-elasticsearch", Required = false, HelpText = "Elasticsearch connection URL.")]
    public string ElasticsearchUrl { get; set; }

    [Option("recreate-postgres-tables", Required = false, HelpText = "Drop the postgres database and recreate it")]
    public bool RecreatePostgresTables { get; set; }

    [Option("elasticsearch-api-key", Required = false, HelpText = "Elasticsearch API Key.")]
    public string ElasticsearchApiKey { get; set; }

    [Option("bulk-elasticsearch", Required = false, HelpText = "Use bulk import method for Elasticsearch.")]
    public bool BulkElasticsearch { get; set; }

    [Option("bulk-postgres", Required = false, HelpText = "Use bulk import method for PostgreSQL.")]
    public bool BulkPostgres { get; set; }

    [Option("skip-postgres", Required = false, HelpText = "Skip populating PostgreSQL database.")]
    public bool SkipPostgres { get; set; }

    [Option("skip-elasticsearch", Required = false, HelpText = "Skip populating Elasticsearch.")]
    public bool SkipElasticsearch { get; set; }
}
