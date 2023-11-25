
# VGStandard

## VGStandard.Data.Importer

### Overview
VGStandard Data Importer is a .NET 8 application designed to import video game-related data into PostgreSQL and Elasticsearch. It uses various technologies like AutoMapper, CommandLine, EFCore.BulkExtensions, and more to handle data transformations and bulk operations efficiently.

Getting Started
Prerequisites
- .NET 8 SDK
- PostgreSQL
- Elasticsearch
- Docker (optional for running PostgreSQL and Elasticsearch)


### Installation
Clone the repository or download the source code.
Ensure PostgreSQL and Elasticsearch are running on your machine or use Docker to spin up containers.

## Recommended Docker Compose
```
git clone https://github.com/DakotaIrsik/VGStandard.git
cd VGStandard
docker-compose up
navigate to -> http://localhost:5001/swagger/index.html
```

#### Running Elasticsearch with Docker
```
docker pull elasticsearch:7.10.1
docker run -d --name elasticsearch -p 9200:9200 -p 9300:9300 -e "discovery.type=single-node" elasticsearch:7.10.1
```
#### Running PostgreSQL with Docker
```
docker pull postgres
docker run --name postgres -e POSTGRES_PASSWORD=yourpassword -d -p 5432:5432 postgres
```
## Configuration

Specify the following run parameters after executing 
```
dotnet build VGStandard.Data.Importer.csproj
```
Update appsettings.json with the correct PostgreSQL and Elasticsearch connection strings.

Running the Importer
The VGStandard Data Importer supports various command-line options for custom execution:
```
--fill-postgres: Specifies the PostgreSQL connection string.
--fill-elasticsearch: Specifies the Elasticsearch URL.
--recreate-postgres-tables: Drops and recreates the PostgreSQL database.
--elasticsearch-api-key: Specifies the API key for Elasticsearch.
--bulk-elasticsearch: Enables bulk import for Elasticsearch.
--bulk-postgres: Enables bulk import for PostgreSQL.
--skip-postgres: Skips populating PostgreSQL database.
--skip-elasticsearch: Skips populating Elasticsearch.
```

## Example Run Commands
```
(If your appsettings.json isn't included, full CLI support)
dotnet run VGStandard.Data.Importer.dll --skip-postgres --fill-elasticsearch="YourElasticsearchUrl" --elasticsearch-api-key="YourApiKey"

(Full Elasticsearch && Postgres install - default command (put connection information into appsettings.json))
dotnet run VGStandard.Data.Importer.dll --bulk-elasticsearch --bulk-postgres
```

### Recommended Docker Compose
```
git clone https://github.com/DakotaIrsik/VGStandard.git
cd VGStandard
docker-compose up
navigate to -> http://localhost:5001/swagger/index.html
```
![image](https://github.com/DakotaIrsik/VGStandard/assets/26256978/ccd7d7df-aa2f-4b74-b548-bc9daf160667)


Contributions, issues, and feature requests are welcome. Feel free to check issues page if you want to contribute.
