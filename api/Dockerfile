# Estágio 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copiar csproj e restaurar dependências
COPY *.csproj ./
RUN dotnet restore

# Copiar tudo e compilar
COPY . ./
RUN dotnet publish -c Release -o out

# Estágio 2: Produção (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /App/out .

# Configurar porta 8080 para a API no Docker
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "RhFacil.Api.dll"]
