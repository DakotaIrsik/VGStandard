using DataImporter.Models;
using Nest;
using Newtonsoft.Json;
using Npgsql;
using System.Data;

namespace DataImporter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Set default values
            string elasticsearchConnectionString = "http://localhost:9200";
            string postgresConnectionString = "Host=localhost;Username=postgres;Password=postgres;Database=videogames;Pooling=false;Port=5432;Timeout=30;";

            // Override defaults if command-line arguments are provided
            foreach (var arg in args)
            {
                if (arg.StartsWith("--fill-elasticsearch="))
                {
                    elasticsearchConnectionString = arg.Substring("--fill-elasticsearch=".Length);
                }
                else if (arg.StartsWith("--fill-postgres="))
                {
                    postgresConnectionString = arg.Substring("--fill-postgres=".Length);
                }
            }

            // Proceed with using the connection strings
            if (!string.IsNullOrEmpty(elasticsearchConnectionString))
            {
                await PopulateElastic(elasticsearchConnectionString);
            }

            if (!string.IsNullOrEmpty(postgresConnectionString))
            {
                await PopulatePostgres(postgresConnectionString);
            }
        }

        static async Task PopulatePostgres(string connectionString)
        {
            await InsertDataFromJson<Region>("Regions", "regions.json", connectionString);
            await InsertDataFromJson<Release>("Releases", "releases.json", connectionString);
            await InsertDataFromJson<Rom>("Roms", "roms.json", connectionString);
            await InsertDataFromJson<GameSystem>("Systems", "systems.json", connectionString);
        }

        private static async Task InsertDataFromJson<T>(string tableName, string filePath, string connectionString) where T : class
        {
            var jsonArray = JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(filePath));
            const int batchSize = 1000; // Adjust based on your data size and database capacity

            using (var conn = new NpgsqlConnection(connectionString))
            {
                await conn.OpenAsync();

                var createTableQuery = GetCreateTableQuery<T>(tableName);
                using (var cmd = new NpgsqlCommand(createTableQuery, conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }

                var columns = typeof(T).GetProperties().Select(p => p.Name).ToArray();
                var columnNames = string.Join(", ", columns);

                for (int i = 0; i < jsonArray.Count; i += batchSize)
                {
                    var batch = jsonArray.Skip(i).Take(batchSize).ToList();

                    using (var writer = conn.BeginBinaryImport($"COPY public.{tableName} ({columnNames}) FROM STDIN (FORMAT BINARY)"))
                    {
                        foreach (var item in batch)
                        {
                            writer.StartRow();
                            foreach (var prop in typeof(T).GetProperties())
                            {
                                writer.Write(prop.GetValue(item));
                            }
                        }

                        await writer.CompleteAsync();
                    }

                    Console.WriteLine($"Inserted {i + batch.Count} records into {tableName}.");
                }
            }

            Console.WriteLine($"Finished inserting data for {tableName}.");
        }


        static string GetCreateTableQuery<T>(string tableName)
        {
            var typeToSqlMapping = new Dictionary<Type, string>
            {
                { typeof(string), "TEXT" },
                { typeof(int), "INTEGER" },
                { typeof(long), "BIGINT" },
                { typeof(bool), "BOOLEAN" },
                { typeof(DateTime), "TIMESTAMP" },
                // ... add more mappings as needed
            };

            var columns = typeof(T).GetProperties().Select(p =>
            {
                var columnName = p.Name;
                var columnType = typeToSqlMapping.ContainsKey(p.PropertyType) ? typeToSqlMapping[p.PropertyType] : "TEXT"; // default to TEXT
                return $"{columnName} {columnType}";
            });

            // Drop the table if it exists, then create a new one
            return $@"
                    DROP TABLE IF EXISTS {tableName};
                    CREATE TABLE {tableName} ({string.Join(", ", columns)});
                    ";
        }

        static async Task PopulateElastic(string connectionString)
        {
            var settings = new ConnectionSettings(new Uri(connectionString))
              .DefaultIndex("default_index")
              .EnableDebugMode();

            var client = new ElasticClient(settings);
            IndexJsonFile<Region>(client, "Regions", "regions.json");
            IndexJsonFile<Release>(client, "Releases", "releases.json");
            IndexJsonFile<Rom>(client, "Roms", "roms.json");
            IndexJsonFile<GameSystem>(client, "Systems", "systems.json");
        }

        static void IndexJsonFile<T>(IElasticClient client, string indexName, string filePath) where T : class
        {
            var jsonArray = JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(filePath));

            const int batchSize = 1000;  // Or whatever batch size you want.
            for (int i = 0; i < jsonArray.Count; i += batchSize)
            {
                var batch = jsonArray.Skip(i).Take(batchSize).ToList();

                var bulkIndexResponse = client.Bulk(b => b
                    .Index(indexName.ToLower())
                    .CreateMany(batch)
                );

                if (bulkIndexResponse.Errors)
                {
                    foreach (var itemWithError in bulkIndexResponse.ItemsWithErrors)
                    {
                        Console.WriteLine($"Failed to index document {itemWithError.Id}: {itemWithError.Error}");
                    }
                }
            }

            Console.WriteLine($"Finished indexing {indexName}.");
        }
    }
}
