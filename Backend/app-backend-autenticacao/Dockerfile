FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar projetos e restaurar dependências
COPY ["Backend/app-backend-autenticacao/app-backend-autenticacao.api/app-backend-autenticacao.api.csproj", "Backend/app-backend-autenticacao/app-backend-autenticacao.api/"]
COPY ["Backend/app-backend-autenticacao/app-backend-autenticacao.domain/app-backend-autenticacao.domain.csproj", "Backend/app-backend-autenticacao/app-backend-autenticacao.domain/"]
COPY ["Backend/app-backend-autenticacao/app-backend-autenticacao.infrastructure/app-backend-autenticacao.infrastructure.csproj", "Backend/app-backend-autenticacao/app-backend-autenticacao.infrastructure/"]
COPY ["Library/Core_Logs/", "Library/Core_Logs/"]
COPY ["Library/Core_Http/", "Library/Core_Http/"]

RUN dotnet restore "Backend/app-backend-autenticacao/app-backend-autenticacao.api/app-backend-autenticacao.api.csproj"

# Copiar o resto dos arquivos
COPY . .
WORKDIR "/src/Backend/app-backend-autenticacao/app-backend-autenticacao.api"
RUN dotnet build "app-backend-autenticacao.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "app-backend-autenticacao.api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Instalação do certificado de confiança (Segurança 100% Nativa)
USER root
RUN apt-get update && apt-get install -y ca-certificates && rm -rf /var/lib/apt/lists/*
COPY nginx/certs/lista-de-compras.crt /usr/local/share/ca-certificates/lista-de-compras.crt
RUN update-ca-certificates

ENTRYPOINT ["dotnet", "app-backend-autenticacao.api.dll"]
