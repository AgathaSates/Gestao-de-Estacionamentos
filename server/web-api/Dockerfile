# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["web-api/Gestao-de-Estacionamentos.WebApi.csproj", "web-api/"]
COPY ["core/aplicacao/Gestao-de-Estacionamentos.Core.Aplicacao.csproj", "core/aplicacao/"]
COPY ["core/dominio/Gestao-de-Estacionamentos.Core.Dominio.csproj", "core/dominio/"]
COPY ["infraestrutura/orm/Gestao-de-Estacionamentos.Infraestutura.Orm.csproj", "infraestrutura/orm/"]
COPY ["Gestao-de-Estacionamento.Infraestrutura.Conf/Gestao-de-Estacionamento.Infraestrutura.Conf.csproj", "Gestao-de-Estacionamento.Infraestrutura.Conf/"]
COPY ["util/token-generator/Gestao-de-Estacionamento.Util.TokenGenerator.csproj", "util/token-generator/"]
RUN dotnet restore "./web-api/Gestao-de-Estacionamentos.WebApi.csproj"
COPY . .
WORKDIR "/src/web-api"
RUN dotnet build "./Gestao-de-Estacionamentos.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Gestao-de-Estacionamentos.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gestao-de-Estacionamentos.WebApi.dll"]