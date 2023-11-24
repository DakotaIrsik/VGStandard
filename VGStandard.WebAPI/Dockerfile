FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["VGStandard.WebAPI/VGStandard.WebAPI.csproj", "VGStandard.WebAPI/"]
RUN dotnet restore "VGStandard.WebAPI/VGStandard.WebAPI.csproj"

COPY . .
WORKDIR "/src/VGStandard.WebAPI"
RUN dotnet build "VGStandard.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VGStandard.WebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VGStandard.WebAPI.dll"]
