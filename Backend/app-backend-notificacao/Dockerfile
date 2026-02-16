FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar projetos e restaurar dependências
COPY ["Backend/app-backend-notificacao/app-backend-notificacao.api/app-backend-notificacao.api.csproj", "Backend/app-backend-notificacao/app-backend-notificacao.api/"]
COPY ["Backend/app-backend-notificacao/app-backend-notificacao.domain/app-backend-notificacao.domain.csproj", "Backend/app-backend-notificacao/app-backend-notificacao.domain/"]
COPY ["Backend/app-backend-notificacao/app-backend-notificacao.infrastructure/app-backend-notificacao.infrastructure.csproj", "Backend/app-backend-notificacao/app-backend-notificacao.infrastructure/"]
COPY ["Library/Core_Logs/Core_Logs.csproj", "Library/Core_Logs/"]
COPY ["Library/Core_Http/Core_Http.csproj", "Library/Core_Http/"]

RUN dotnet restore "Backend/app-backend-notificacao/app-backend-notificacao.api/app-backend-notificacao.api.csproj"

# Copiar o resto dos arquivos
COPY . .
WORKDIR "/src/Backend/app-backend-notificacao/app-backend-notificacao.api"
RUN dotnet build "app-backend-notificacao.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "app-backend-notificacao.api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Instalação do certificado de confiança (Segurança 100% Nativa)
USER root
RUN apt-get update && apt-get install -y ca-certificates && rm -rf /var/lib/apt/lists/*
COPY nginx/certs/lista-de-compras.crt /usr/local/share/ca-certificates/lista-de-compras.crt
RUN update-ca-certificates

ENTRYPOINT ["dotnet", "app-backend-notificacao.api.dll"]
