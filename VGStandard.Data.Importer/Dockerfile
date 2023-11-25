FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["VGStandard.Data.Importer/VGStandard.Data.Importer.csproj", "VGStandard.Data.Importer/"]
RUN dotnet restore "VGStandard.Data.Importer/VGStandard.Data.Importer.csproj"

COPY . .
WORKDIR "/src/VGStandard.Data.Importer"
RUN dotnet build "VGStandard.Data.Importer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VGStandard.Data.Importer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

COPY ["VGStandard.Data.Importer/Roms.json", "/app/roms.json"]
COPY ["VGStandard.Data.Importer/Releases.json", "/app/releases.json"]
COPY ["VGStandard.Data.Importer/Regions.json", "/app/regions.json"]
COPY ["VGStandard.Data.Importer/Systems.json", "/app/systems.json"]

COPY ["VGStandard.Data.Importer/ElasticRoms.json", "/app/ElasticRoms.json"]
COPY ["VGStandard.Data.Importer/ElasticReleases.json", "/app/ElasticReleases.json"]
COPY ["VGStandard.Data.Importer/ElasticRegions.json", "/app/ElasticRegions.json"]
COPY ["VGStandard.Data.Importer/ElasticSystems.json", "/app/ElasticSystems.json"]

ENTRYPOINT ["dotnet", "VGStandard.Data.Importer.dll"]
