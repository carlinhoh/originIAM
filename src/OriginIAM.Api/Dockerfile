FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/OriginIAM.Api/OriginIAM.Api.csproj", "src/OriginIAM.Api/"]
RUN dotnet restore "src/OriginIAM.Api/OriginIAM.Api.csproj"
COPY . .
WORKDIR "/src/OriginIAM.Api"
RUN dotnet build "OriginIAM.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OriginIAM.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["src/OriginIAM.Api/appsettings.json", "/app/"]
ENTRYPOINT ["dotnet", "OriginIAM.Api.dll"]
