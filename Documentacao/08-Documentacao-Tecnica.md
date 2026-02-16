# 📘 Documentação Técnica e Arquitetura

---

## 1. Arquitetura do Sistema

O sistema utiliza uma arquitetura de **Microserviços Desacoplados** via **Library Core pattern**.

### Componentes

- **Core_Logs**: Centralização de logs consolidados e segurança.
- **Core_Http**: Abstração de chamadas resilientes.
- **Microserviços**: Autenticação, Produtos, Segurança.
- **Gateway**: Unificação de rotas via ASP.NET Core Minimal APIs.

## 2. Dependências de Infraestrutura

| Backend | SQL Server (BD) | Redis (Cache) | MongoDB |
| :--- | :---: | :---: | :---: |
| **Autenticação** | ✅ (AuthDb) | ❌ | ❌ |
| **Produto** | ✅ (ProdutoDB) | ✅ | ❌ |
| **Segurança** | ❌ (Stateless) | ❌ | ❌ |

## 3. Stack Tecnológica

- **Backend**: .NET 8, EF Core, MediatR, Mapster.
- **Frontend**: Angular 21 (Material Design), Ionic 8 (Capacitor).
- **Persistência**: SQL Server (Principal), Redis (Cache).
- **Comunicação**: HTTP/JSON, Kafka (Logs).

## 3. Diagrama de Fluxo (Simplificado)

`Usuário -> Gateway -> Auth Service (Token) -> Outros Microserviços`

## 4. Padrões de Código

- Clean Architecture.
- Standalone Components (Frontend).
- Handlers/Commands Pattern (Backend).

## 5. Convenções e Regras de Build (Frontend e Docker)

- **Pasta de funcionalidades**: nome fixo **funcionalidades** (não "features"); imports `./funcionalidades/...`.
- **Módulos Angular**: imports `./app-routing.module` e `./app.module`; `main.ts` usa `./app/app.module`.
- **Standalone**: componentes standalone não vão em `declarations`; apenas nas rotas.
- **Projeto Angular completo**: deve ter `src/main.ts`, `src/index.html`, `src/styles.scss` e `public/` se referenciado no angular.json.
- **TypeScript**: em apps com RxJS 7 + TS 5.9+, usar `noImplicitOverride: false` no tsconfig.
- **package.json (Docker)**: não usar `packageManager` em frontends que buildam em Node 20; auth-frontend usa `npm ci` com package-lock.
- **Path alias para Library**: módulos que importam `@app/logs` (../../Library) precisam de build Docker com **context na raiz do repo** e cópia de `Frontend/Library`.
- **Specs**: componentes standalone nos testes usam `imports: [Componente]` e nome da classe correto (ex.: `ListasComponent`).

Regras para o agente: ver **Documentacao/14-Regras-Agente.md** (copiar para Cursor > Settings > Rules for AI).
