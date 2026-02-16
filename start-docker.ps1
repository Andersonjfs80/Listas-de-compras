# Script para subir o ambiente em etapas
Write-Host "==> Etapa 1: Subindo infraestrutura (SQL + Redis)..." -ForegroundColor Cyan
docker-compose up -d sqlserver redis

Write-Host "`n==> Aguardando 10 segundos..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

Write-Host "`n==> Etapa 2: Buildando e subindo backends..." -ForegroundColor Cyan
docker-compose up -d --build auth-api produto-api seguranca-api

Write-Host "`n==> Aguardando 15 segundos..." -ForegroundColor Yellow
Start-Sleep -Seconds 15

Write-Host "`n==> Etapa 3: Subindo gateway..." -ForegroundColor Cyan
docker-compose up -d --build produto-gateway auth-gateway

Write-Host "`n==> Aguardando 10 segundos..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

Write-Host "`n==> Etapa 4: Subindo frontends..." -ForegroundColor Cyan
docker-compose up -d --build angular-app ionic-app auth-frontend

Write-Host "`n==> Verificando status dos containers..." -ForegroundColor Green
docker-compose ps

Write-Host "`n==> Ambiente completo! Acesse:" -ForegroundColor Green
Write-Host "  - Angular: http://localhost:4200" -ForegroundColor White
Write-Host "  - Ionic: http://localhost:8100" -ForegroundColor White
Write-Host "  - Gateway: http://localhost:5000" -ForegroundColor White
