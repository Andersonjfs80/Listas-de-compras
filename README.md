# 🛒 Projeto Listas de Compras - Ecossistema Monorepo

Bem-vindo ao projeto **Listas de Compras**. Este é um ecossistema completo e resiliente, organizado em um modelo de **Monorepo** para facilitar a governança, mas projetado com microserviços independentes para escalabilidade e estudo.

## 🐳 Como Rodar o Projeto (Docker)

Todo o ecossistema (Backends, Frontends, Gateway e Infraestrutura) está conteinerizado.

### 🚀 Comando Principal

Para subir todas as peças do projeto pela primeira vez (ou após mudanças no código):

```bash
docker-compose up -d --build
```

> [!IMPORTANT]
> **Por que pode "travar" ou demorar?**
> No primeiro build, o Docker precisa baixar as imagens base do .NET e Node, além de executar o `pnpm install` no Angular e o `dotnet restore` nos backends. Isso pode levar alguns minutos dependendo da sua conexão e hardware. Acompanhe os logs se necessário com `docker-compose logs -f`.

### 🔑 Credenciais de Desenvolvimento

- **SQL Server (SA)**: `Dev@123456`
- **Portas Mapeadas**:
  - **Gateway API**: 5000
  - **Angular App**: 4200
  - **Ionic App**: 8100
  - **Auth API**: 5001
  - **Produto API**: 5002
  - **Segurança API**: 5003
  - **SQL Server**: 1433
  - **Redis**: 6379

### 🔒 HTTPS & Segurança

Todo o ambiente está configurado para rodar com **HTTPS** via certificados autoassinados.

- **Certificados**: Localizados em `./nginx/certs` (gerados via OpenSSL).
- **Acesso**: Ao acessar `https://localhost`, aceite o alerta de segurança do navegador.
- **Detalhes**: Consulte o manual completo em [HTTPS_GUIDE.md](./HTTPS_GUIDE.md).

---

## ✨ O que fizemos recentemente?

### 1. 🏗️ Infraestrutura Resiliente

- **Docker Compose Mestre**: Orquestração completa de todas as peças.
- **SQL Server, MongoDB, Redis e Elastic Stack**: Configurados com **volumes persistentes**, garantindo que seus dados não sumam ao reiniciar os containers.

### 2. 🧠 Inteligência de Inicialização & Massa de Dados

- **Auto-criação de Banco**: Os backends (Produto e Autenticação) criam o banco de dados e as tabelas automaticamente se eles não existirem.
- **Seeding de Dados**: Em ambiente de `Development`, o sistema gera automaticamente uma lista de **10 produtos** (Arroz, Feijão, etc.) com categorias, preços e imagens para você começar a testar imediatamente.

### 3. 📸 Gestão Avançada de Imagens

- **Modelo de Imagens**: Implementamos um sistema de galeria onde cada produto pode ter múltiplas fotos.
- **Flexibilidade Total**: O campo `Conteudo` aceita tanto **URLs** externas quanto strings **Base64** (armazenamento `nvarchar(max)` no SQL).
- **Atributos de Negócio**: Controle de imagem Principal/Adicional, Favoritos e Exclusão Lógica.

### 4. 📂 Governança e Documentação

- **Pasta `Documentacao`**: Centralização de todos os artefatos de gestão do projeto (Project Charter, EAP, Cronograma, etc.).
- **Padrões de Agente**: Manuais de IA atualizados para garantir que novos desenvolvedores (humanos ou IAs) sigam os padrões de nomenclatura e arquitetura estabelecidos.

### 5. 📦 Migração para pnpm

- **Gerenciador**: Todos os projetos frontend migraram de **npm** para **pnpm** v10+.
- **Lockfiles**: Cada pod possui seu próprio `pnpm-lock.yaml` garantindo builds reproduzíveis.
- **Docker**: Dockerfiles atualizados para usar `pnpm install --frozen-lockfile` e `pnpm run build`.
- **Comandos**: Veja o guia completo em [COMANDOS.md](./Documentacao/COMANDOS.md).

---

## 🛠️ Tecnologias Utilizadas

- **Backend**: .NET 8, EF Core, MediatR, Mapster.
- **Frontend**: Angular 21+, Micro-Frontends (Pods independentes por porta Docker).
- **Gerenciador de Pacotes (Frontend)**: **pnpm** v10+ (mais rápido e eficiente que npm).
- **Gateway**: Custom Gateway (.NET 8 Minimal APIs).
- **DevOps**: Docker, Docker Compose, Git.

---
---
*Este projeto foi desenvolvido seguindo os padrões de Clean Architecture e Clean Code.*

---

### 📚 Documentação Útil

- [Comandos do Projeto](./Documentacao/COMANDOS.md) - Guia de Git, Docker e Desenvolvimento.
- [URLs e Endpoints](./Documentacao/URLS.md) - Onde acessar cada peça do sistema.
- [Exemplos de cURL](./Documentacao/CURLS.md) - Comandos para testar as APIs manualmente.
- [Análise Técnica (Gateway/Guards)](./Documentacao/ANALISE_TECNICA.md) - Estudo sobre Ocelot e bibliotecas de validação.
- [Guia HTTPS](./HTTPS_GUIDE.md) - Detalhes sobre segurança e certificados.
