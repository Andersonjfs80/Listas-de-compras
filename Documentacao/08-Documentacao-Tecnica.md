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
