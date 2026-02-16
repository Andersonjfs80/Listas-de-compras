# Regras do agente (Rules for AI) - Consolidado 2026

Copie o bloco abaixo e cole em **Cursor > Settings > Rules for AI**.

---

```
Neste projeto (Listas de compras):

- **INICIALIZAÇÃO OBRIGATÓRIA**: Ao iniciar o Cursor ou qualquer nova interação, o agente **DEVE** ler todos os padrões na pasta `../Agentes IA`.
- **IDIOMA E COMUNICAÇÃO**: TODAS as respostas e documentação em PORTUGUÊS (PT-BR).
- **ESTRUTURA ANGULAR (FRONTEND GLOBAL)**:
  - Pasta de funcionalidades: sempre "modulos", nunca "features". Imports: `./modulos/...`
  - Micro-Frontends: Módulos independentes (Auth: 4201, Home: 4202, Main: 4200).
  - Integração Mobile: usa iframe-wrapper carregando pods via porta Docker.
  - Standalone: Componentes wrapper podem ser standalone: false se declarados em Module. LoadComponent nas rotas.
  - RxJS/TS: noImplicitOverride: false no tsconfig.json.
  - Docker: node:22-alpine para frontends com npm 11. Context na raiz se importar @app/logs.

- **ESTRUTURA .NET (BACKEND GLOBAL)**:
  - Padrão: Clean Architecture + CQRS + MediatR + Mapster.
  - Camadas: Api (Controllers/Startup), Domain (Models, Commands, Handlers, Interfaces), Infrastructure (Repo, Mappings, IoC).
  - Nomenclatura: Sufixo "Model" para domínio. Sem sufixos redundantes em projetos (-aut, -gtw).
  - IoC: Sempre em Infrastructure/IoC/DependencyInjection.cs.
  - Mapster: Usar .Adapt<T>() diretamente, nunca injetar IMapper. Scan automático em Infrastructure/Mappings.
  - Program.cs: Deve ser LIMPO. Lógica de inicialização em extension methods na Infrastructure.
  - Swagger: Caminhos relativos (v1/swagger.json). PathBase idêntico ao serviço no appsettings.

- **HEADERS E SEGURANÇA (PADRÃO 2026)**:
  - MANDATÓRIOS: SIGLA-APLICACAO-MODULO e MESSAGE-ID-MODULO (específicos por pod), SESSAO-ID, HARDWARE-ID via Interceptor da `app-library-logs`.
  - TIMEOUT FRONTEND: Padrão global de 30 segundos (30000ms) implementado via `TimeoutInterceptor`.
  - SESSÃO & REDIS: O módulo de autenticação gera e persiste a sessão no Redis com a "assinatura" definida.
  - SEM MOCKS: Nunca usar mocks em validações de hardware/segurança que devam vir do cache/gateway.
  - CENTRALIZAÇÃO: Login EXCLUSIVAMENTE via app-modulo-autenticacao. Lógica de interceptação centralizada em biblioteca global.

- **OBSERVABILIDADE E MANUTENÇÃO**:
  - KAFKA: Prioridade total para logs consolidados via Kafka Background Service. Uso DIRETO de IKafkaLogger, sem handlers extras ou MediatR para infra de log.
  - GATEWAY: GatewayExtensions deve tratar tipos anuláveis (CS8625) e validar headers mandatórios.
  - OCORRÊNCIAS: Centralizar bugs e pendências na raiz em OCORRENCIAS_E_BUGS.md.

- **DISCIPLINA OPERACIONAL (CRÍTICO)**:
  - PLANO DE TRABALHO: NUNCA crie infraestruturas, handlers ou classes não solicitadas explicitamente no plano ("lixo").
  - REVISÃO: Antes de criar qualquer novo componente de infra, valide no plano com o usuário.
  - ABREVIAÇÃO ZERO: PROIBIDO o uso de nomes abreviados em rotas, pastas, projetos ou classes. Ex: "Auth" -> "Autenticacao", "Prod" -> "Produto", "Gtw" -> "Gateway".
  - ROTAS: NUNCA deixar rotas vazias `[Route("")]`. Sempre usar o nome completo do recurso.
```

---

Atualize as Rules for AI quando houver novas convenções.
