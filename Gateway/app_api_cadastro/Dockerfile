FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar projetos e restaurar dependências
COPY ["Gateway/app_api_cadastro/app-api-cadastro.csproj", "Gateway/app_api_cadastro/"]
COPY ["Library/Core_Logs/", "Library/Core_Logs/"]
COPY ["Library/Core_Http/", "Library/Core_Http/"]

RUN dotnet restore "Gateway/app_api_cadastro/app-api-cadastro.csproj"

# Copiar o resto dos arquivos
COPY . .
WORKDIR "/src/Gateway/app_api_cadastro"
RUN dotnet build "app-api-cadastro.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "app-api-cadastro.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Instalação do certificado de confiança (Segurança 100% Nativa)
USER root
RUN apt-get update && apt-get install -y ca-certificates && rm -rf /var/lib/apt/lists/*
COPY certificados/lista-de-compras.crt /usr/local/share/ca-certificates/lista-de-compras.crt
RUN update-ca-certificates

ENTRYPOINT ["dotnet", "app-api-cadastro.dll"]
