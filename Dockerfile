# Define a base do SDK do .NET para o build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copia todos os projetos .csproj e restaura as dependências
COPY ["src/OriginIAM.Api/OriginIAM.Api.csproj", "OriginIAM.Api/"]
COPY ["src/OriginIAM.Application/OriginIAM.Application.csproj", "OriginIAM.Application/"]
COPY ["src/OriginIAM.Domain/OriginIAM.Domain.csproj", "OriginIAM.Domain/"]
COPY ["src/OriginIAM.Infrastructure/OriginIAM.Infrastructure.csproj", "OriginIAM.Infrastructure/"]

RUN dotnet restore "OriginIAM.Api/OriginIAM.Api.csproj"
RUN dotnet restore "OriginIAM.Application/OriginIAM.Application.csproj"
RUN dotnet restore "OriginIAM.Domain/OriginIAM.Domain.csproj"
RUN dotnet restore "OriginIAM.Infrastructure/OriginIAM.Infrastructure.csproj"

# Copia o restante dos arquivos de código fonte
COPY src .

# Muda o diretório de trabalho para o projeto da API e constrói a aplicação
WORKDIR /src/OriginIAM.Api
RUN dotnet build -c Release -o /app/build

# Publica o projeto
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Define a imagem base para a execução da aplicação
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OriginIAM.Api.dll"]
