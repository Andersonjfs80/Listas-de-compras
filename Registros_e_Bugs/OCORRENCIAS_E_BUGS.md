# 🐛 Registro de Bugs, Logs e Pendências Técnicas

Este documento centraliza a identificação de bugs, análises de logs e pendências técnicas para facilitar a manutenção e a colaboração com o agente.

---

## 📅 14/02/2026

### 1. BUG: Cabeçalhos Obrigatórios Ausentes (MISSING_MANDATORY_HEADERS)

- **Status**: 🟢 RESOLVIDO
- **Resumo**: Implementados `HeaderInterceptor` nos módulos `app-modulo-autenticacao` e `app-modulo-home`.
- **Efeito**: Headers `SIGLA-APLICACAO`, `SESSAO-ID`, `MESSAGE-ID` e `HARDWARE-ID` agora são injetados automaticamente em todas as requisições HttpClient.

### 2. Pendência: Centralização do Login (Light Login vs Auth Module)

- **Status**: 🟢 RESOLVIDO
- **Resumo**: Login removido do módulo Home. `authGuard` do Home configurado para redirecionar para `http://localhost:4201` (Auth Pod).
- **Efeito**: Fluxo de autenticação agora é único e centralizado.

### 3. Integração com Redis para Sessões

- **Status**: 🟢 RESOLVIDO
- **Resumo**: Backend de autenticação (`app-backend-autenticacao`) configurado com Redis.
- **Efeito**: O `LoginHandler` agora persiste a "Assinatura" da sessão (`Auth:Session:{SessionId}`) contendo dados do usuário e `HardwareId` no cache Redis.

### 4. Boas Práticas e Regras do Agente

- **Status**: 🟢 CONCLUÍDO
- **Resumo**: Regras globais de Frontend e Backend consolidadas em `Documentacao/14-Regras-Agente.md`.
- **Efeito**: Novos projetos seguirão os padrões aprendidos, evitando reincidência nos mesmos erros técnicos.

### 5. Organização de Estrutura e Limpeza de Logs

- **Status**: 🟢 CONCLUÍDO
- **Resumo**: Limpeza profunda da raiz realizada. Todos os arquivos de log, erro e backups temporários (`all_errors.txt`, `build_errors_detailed.log`, `build_revert_errors.log`, `core_logs_build.log`, `errors_utf8_2.log`, `docker-compose BK.yml`, etc.) foram movidos para `Registros_e_Bugs`.
- **Efeito**: Raiz do projeto 100% limpa e organizada.

---

## 🚀 Próximos Passos

- Validar a propagação do `token` entre os pods via `localStorage` compartilhado (ou redirecionamento com parâmetro).
- Monitorar logs no Kafka para garantir que todos os microserviços recebam os novos headers sem erros de validação.
