#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["VGStandard.DataImporter/VGStandard.DataImporter.csproj", "VGStandard.DataImporter/"]
COPY ["VGStandard.Common.Web/VGStandard.Common.Web.csproj", "VGStandard.Common.Web/"]
COPY ["VGStandard.Core/VGStandard.Core.csproj", "VGStandard.Core/"]
RUN dotnet restore "./VGStandard.DataImporter/./VGStandard.DataImporter.csproj"
COPY . .
WORKDIR "/src/VGStandard.DataImporter"
RUN dotnet build "./VGStandard.DataImporter.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./VGStandard.DataImporter.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VGStandard.DataImporter.dll"]