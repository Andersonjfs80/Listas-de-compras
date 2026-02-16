# Registro de Refatorações Técnicas e Correções (13/02/2026)

Este documento detalha as mudanças técnicas, refatorações e correções aplicadas ao sistema no dia 13 de Fevereiro de 2026, focando no backend de Produto, integração com Kafka e autenticação no Frontend.

## 1. Problema: Estabilidade do Log Kafka e Docker DNS

### Contexto: Instabilidade de Logs e Conexão em Dev

O serviço `app-backend-autenticacao` e `produto` estavam crashando com erro 139 ou entrando em loop infinito de log.

- **Causa 1**: `KafkaBackgroundService` tentava enviar logs mesmo com `KafkaSettings.Enabled = false`, gerando `ProduceException`. O catch gerava novo log de erro, criando loop.
- **Causa 2**: Erro `Name or service not known` ao conectar no SqlServer durante a inicialização do container, devido a race condition de DNS no Docker.

### Solução Aplicada

1. **Infraestrutura Kafka**: Adicionados serviços `zookeeper` e `kafka` ao `docker-compose.yml` para prover um ambiente de mensageria local completo.
2. **KafkaLogger.cs**: Corrigida a inicialização do `Producer`. Agora ele é instanciado apenas se `Enabled = true`, evitando erros de conexão prematuros.
3. **Ativação nos Backends**: Kafka habilitado nos arquivos `appsettings.json` de Autenticação e Produto, apontando para o broker interno (`kafka:29092`).
4. **Resiliência DB**: Implementado padrão **Retry** na inicialização do `DbInitializer` para suportar delays de DNS no Docker.

---

## 2. Refatoração: Relacionamento N-N Produto-Categoria

### Contexto: Mudança para Muitos-para-Muitos e Padronização de Tabelas

O modelo original `ProdutoModel` tinha uma FK direta `CategoriaId` (1-N). Foi identificada a necessidade de um produto pertencer a múltiplas categorias (Principal e Adicionais). Além disso, a nomenclatura de tabelas estava inconsistente (`Precos` vs `ProdutoPrecos`).

### Mudanças no Domínio

1. **ProdutoModel**:
    - Removido `CategoriaId` e prop `Categoria`.
    - Adicionado `ICollection<ProdutoCategoriaModel> ProdutoCategorias`.
    - Renomeadas coleções: `Precos` -> `ProdutoPrecos`, `Codigos` -> `ProdutoCodigos`, `Imagens` -> `ProdutoImagens`.
2. **CategoriaModel**:
    - Renomeado auto-relacionamento: `OwnerCategoriaId` -> `OwnerId`.
3. **Novo Modelo**: `ProdutoCategoriaModel` (Junction Table).
    - Chave Composta: `ProdutoId` + `CategoriaId`.
    - Enum `TipoCategoria`: `Principal`, `Adicional`.

### Mudanças na Infraestrutura

1. **AppDbContext**: Mapeamento via Fluent API para N-N.
2. **Migrations**: Criada migration `RefactorProdutoCategoria` removendo colunas antigas e criando tabela de junção.
3. **ProdutoRepository**:
    - Atualizados métodos `ObterPorId`, `ObterTodos`, `ObterComPaginacao` para usar `.Include(p => p.ProdutoCategorias).ThenInclude(pc => pc.Categoria)`.
    - Filtros de consulta atualizados para `p.ProdutoCategorias.Any(pc => pc.CategoriaId == id)`.
4. **DbInitializer**: Atualizado Seed para criar produtos vinculando categorias via `ProdutoCategoriaModel` (Tipo Principal).

---

## 3. Frontend: Módulo de Autenticação

### Contexto: Checkbox Lembrar-me e Provisionamento Docker

Necessidade de funcionalidade "Lembrar-me" no login para salvar o identificador do usuário (email/cpf). Além disso, o módulo de autenticação não estava listado no `docker-compose.yml`.

### Solução: Estratégia de Provisionamento e Mudanças UI

1. **Docker**: Adicionado serviço `app-modulo-autenticacao` no `docker-compose.yml` (Porta 4204:80).
2. **LoginComponent**:
    - Adicionado `MatCheckbox` "Lembrar-me".
    - Lógica no `ngOnInit` para carregar `savedIdentifier` do `localStorage`.
    - Lógica no `onSubmit` para salvar/remover `savedIdentifier` baseada no checkbox.
3. **Layout**: Ajustes de CSS para alinhar checkbox e link "Esqueci senha".

---

## 4. Status Atual dos Serviços

| Serviço | Porta Docker | Status | Observação |
| :--- | :--- | :--- | :--- |
| `sqlserver` | 1433 | OK | |
| `redis_cache` | 6379 | OK | |
| `app-backend-produto` | 6002 | OK | N-N Refatorado |
| `app-backend-autenticacao` | 7000 | OK | Fix Kafka Loop |
| `app-modulo-autenticacao` | 4204 | OK | Lembrar-me add |
| `app-modulo-home` | 4202 | OK | |
| `app-mobile-compras` | 4200 | OK | |

---

## Próximos Passos Sugeridos

1. Implementar endpoints de **Criação/Edição de Produto** no Backend (atualmente inexistentes ou quebrados pela mudança de modelo).
2. Validar fluxo completo de Login -> Home -> Listagem de Produtos no ambiente Docker integrado.
