FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar projetos e restaurar dependências
COPY ["Backend/app-backend-lista-compras/app-backend-lista-compras.api/app-backend-lista-compras.api.csproj", "Backend/app-backend-lista-compras/app-backend-lista-compras.api/"]
COPY ["Backend/app-backend-lista-compras/app-backend-lista-compras.domain/app-backend-lista-compras.domain.csproj", "Backend/app-backend-lista-compras/app-backend-lista-compras.domain/"]
COPY ["Backend/app-backend-lista-compras/app-backend-lista-compras.infrastructure/app-backend-lista-compras.infrastructure.csproj", "Backend/app-backend-lista-compras/app-backend-lista-compras.infrastructure/"]
COPY ["Library/Core_Logs/", "Library/Core_Logs/"]
COPY ["Library/Core_Http/", "Library/Core_Http/"]

RUN dotnet restore "Backend/app-backend-lista-compras/app-backend-lista-compras.api/app-backend-lista-compras.api.csproj"

# Copiar o resto dos arquivos
COPY . .
WORKDIR "/src/Backend/app-backend-lista-compras/app-backend-lista-compras.api"
RUN dotnet build "app-backend-lista-compras.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "app-backend-lista-compras.api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Instalação do certificado de confiança (Segurança 100% Nativa)
USER root
RUN apt-get update && apt-get install -y ca-certificates && rm -rf /var/lib/apt/lists/*
COPY certificados/lista-de-compras.crt /usr/local/share/ca-certificates/lista-de-compras.crt
RUN update-ca-certificates

ENTRYPOINT ["dotnet", "app-backend-lista-compras.api.dll"]
