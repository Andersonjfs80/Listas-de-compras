# Regras do agente (Rules for AI) - Consolidado 2026

Copie o bloco abaixo e cole em **Cursor > Settings > Rules for AI**.

---

```text
Neste projeto (Listas de compras):

- **INICIALIZAÇÃO OBRIGATÓRIA**: Ao iniciar o Cursor ou qualquer nova interação, o agente **DEVE** ler todos os padrões na pasta `../Agentes IA`.
- **IDIOMA E COMUNICAÇÃO**: TODAS as respostas e documentação em PORTUGUÊS (PT-BR).
- **ESTRUTURA ANGULAR (FRONTEND GLOBAL)**:
  - Pasta de funcionalidades: sempre "modulos", nunca "features". Imports: `./modulos/...`
  - Micro-Frontends: Módulos independentes (Auth: 4201, Home: 4202, Main: 4200).
  - Integração Mobile: usa iframe-wrapper carregando pods via porta Docker.
  - Standalone: Componentes wrapper podem ser standalone: false se declarados em Module. LoadComponent nas rotas.
  - RxJS/TS: noImplicitOverride: false no tsconfig.json.
  - Docker: node:22-alpine para frontends com **pnpm**. Instalar via `npm install -g pnpm` e usar `pnpm install --frozen-lockfile`. Context na raiz se importar @app/logs.

- **ESTRUTURA .NET (BACKEND GLOBAL)**:
  - Padrão: Clean Architecture + CQRS + MediatR + Mapster.
  - Camadas: Api (Controllers/Startup), Domain (Models, Commands, Handlers, Interfaces), Infrastructure (Repo, Mappings, IoC).
  - Nomenclatura: Sufixo "Model" para domínio. Sem sufixos redundantes em projetos (-aut, -gtw).
  - IoC: Sempre em Infrastructure/IoC/DependencyInjection.cs.
  - Mapster: Usar .Adapt<T>() diretamente, nunca injetar IMapper. Scan automático em Infrastructure/Mappings.
  - Program.cs: Deve ser LIMPO. Lógica de inicialização em extension methods na Infrastructure.
  - Swagger: Caminhos relativos (v1/swagger.json). PathBase idêntico ao serviço no appsettings.

- **GATEWAYS E ROTEAMENTO**:
  - `app-api-autenticacao` → tudo que envolve login, token, sessão (aponta para `app-backend-autenticacao`).
  - `app-api-cadastro` → **gateway central de cadastro**: produtos, categorias, preços e qualquer novo recurso de negócio. Aponta para `app-backend-produto` e futuramente outros backends de cadastro.
  - `app-backend-lista-compras` → **exceção temporária**: ainda sem gateway dedicado; o nginx do `app-mobile-compras` roteia `/app-api-lista-compras` diretamente para este backend enquanto o módulo amadurece.
  - Regra geral: novos recursos de negócio → entram em `app-api-cadastro`. Autenticação/segurança → `app-api-autenticacao`.

- **HEADERS E SEGURANÇA (PADRÃO 2026)**:
  - MANDATÓRIOS: SIGLA-APLICACAO-MODULO e MESSAGE-ID-MODULO (específicos por pod), SESSAO-ID, HARDWARE-ID via Interceptor da `app-library-logs`.
  - TIMEOUT FRONTEND: Padrão global de 30 segundos (30000ms) implementado via `TimeoutInterceptor`.
  - SESSÃO & REDIS: O módulo de autenticação gera e persiste a sessão no Redis com a "assinatura" definida.
  - SEM MOCKS: Nunca usar mocks em validações de hardware/segurança que deveriam vir do cache/gateway.
  - CENTRALIZAÇÃO: Login EXCLUSIVAMENTE via app-modulo-autenticacao. Lógica de interceptação centralizada em biblioteca global.
  - Criptografia E2E: Uso obrigatório do header `X-Sec-Key: 1` e middleware de descriptografia como primeiro no pipeline.
  - Resiliência de Stream: Em middlewares que lêem o body, sempre resetar `Position = 0` para evitar erros 400 no ModelBinding.
  - Migrations Automatizadas: O banco de dados deve ser atualizado exclusivamente via `context.Database.Migrate()` na inicialização do serviço. NUNCA rodar comandos manuais de banco no ambiente de execução.
  - HISTÓRICO DE SENHAS: Bloquear reuso das últimas 5 senhas. Armazenar em `HistoricoSenhasJson` (Criptografado).
  - EXPIRAÇÃO: Forçar troca de senha a cada 90 dias. Validar via `ISecurityService.GetPasswordStatus`.
  - UX ANTI-DUPLO CLIQUE: Uso mandatório de `LoadingService` para desabilitar botões e inputs durante requisições HTTP (Interceptor).

- **OBSERVABILIDADE E MANUTENÇÃO**:
  - KAFKA: Prioridade total para logs consolidados via Kafka Background Service. Uso DIRETO de IKafkaLogger, sem handlers extras ou MediatR para infra de log.
  - GATEWAY: GatewayExtensions deve tratar tipos anuláveis (CS8625) e validar headers mandatórios.
  - OCORRÊNCIAS: Centralizar bugs e pendências na raiz em `OCORRENCIAS_E_BUGS.md`. Qualquer arquivo de erro gerado no terminal (como `.log`, `build.log`, `error.log`) deve ser **obrigatoriamente** movido para a pasta `OCORRENCIAS_E_BUGS/logs/` e NUNCA deixado espalhado pelas pastas ou na raiz do repositório.

- **DISCIPLINA OPERACIONAL E MANUTENÇÃO (CRÍTICO)**:
  - PLANO DE TRABALHO: NUNCA crie infraestruturas, handlers ou classes não solicitadas explicitamente no plano ("lixo").
  - LIXO E CÓDIGO MORTO: **PROIBIDO** criar arquivos de backup (`.bak`, `.old`, etc.) durante refatorações. **PROIBIDO** deixar blocos inteiros de código antigo comentado. O código deve ser substituído de forma limpa.
  - PADRONIZAÇÃO DE RETORNOS HTTP: Em controladores de backend (`*Controller.cs`) que herdam de `BaseController`, é **OBRIGATÓRIO** utilizar o método `FromCommand(response)` para retornos de requisições, ao invés de usar DIRETAMENTE métodos explícitos do MVC como `Ok()`, `CreatedAtAction()`, `NoContent()`, etc. O `FromCommand` já gerencia o status code através do `BaseCommand`. NÃO altere a tipagem de métodos base nativos do MVC (como tentar forçar o `base.NoContent()` a retornar dados JSON em um `ObjectResult`).
  - REVISÃO: Antes de criar qualquer novo componente de infra, valide no plano com o usuário.
  - ABREVIAÇÃO ZERO: PROIBIDO o uso de nomes abreviados em rotas, pastas, projetos ou classes. Ex: "Auth" -> "Autenticacao", "Prod" -> "Produto", "Gtw" -> "Gateway".
  - ROTAS: NUNCA deixar rotas vazias `[Route("")]`. Sempre usar o nome completo do recurso.
```

---

Atualize as Rules for AI quando houver novas convenções.
