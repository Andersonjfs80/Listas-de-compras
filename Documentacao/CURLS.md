# ⚡ Guia de Comandos cURL

Este guia fornece exemplos de comandos `curl` para testar as APIs do projeto **Listas de Compras**.

> [!NOTE]
> Os exemplos abaixo utilizam `localhost`. Se estiver testando o ambiente Docker via HTTPS, use a flag `-k` ou `--insecure` para ignorar o aviso de certificado autoassinado.

---

## 🔑 Autenticação

### 1. Realizar Login

Obtenha o token para as próximas chamadas.

**Local (Porta 5001):**

```bash
curl -X POST https://localhost:5001/app-api-autenticacao/login \
     -H "Content-Type: application/json" \
     -d '{
           "identificador": "seu_usuario",
           "senha": "sua_senha"
         }' -k
```

**Docker (Nginx):**

```bash
curl -X POST https://localhost/app-api-autenticacao/login \
     -H "Content-Type: application/json" \
     -d '{
           "identificador": "seu_usuario",
           "senha": "sua_senha"
         }' -k
```

### 2. Cadastrar Novo Usuário

**Exemplo:**

```bash
curl -X POST https://localhost/app-api-autenticacao/cadastrar \
     -H "Content-Type: application/json" \
     -d '{
           "nome": "João Silva",
           "email": "joao@email.com",
           "documento": "12345678901",
           "apelido": "joaosilva",
           "senha": "Senha@123"
         }' -k
```

---

## 🛒 Produtos

### 1. Listar Produtos (Requer Token)

Substitua `SEU_TOKEN_AQUI` pelo token retornado no login.

**Docker (Nginx):**

```bash
curl -X GET "https://localhost/app-api-produto/produtos?pageNumber=1&pageSize=10" \
     -H "Token: SEU_TOKEN_AQUI" -k
```

**Filtros Opcionais:**

```bash
curl -X GET "https://localhost/app-api-produto/produtos?nome=Arroz&ordemCrescente=true" \
     -H "Token: SEU_TOKEN_AQUI" -k
```

---

## 🔄 Sessão e Senha

### 1. Logout

```bash
curl -X POST https://localhost/app-api-autenticacao/logout \
     -H "Token: SEU_TOKEN_AQUI" -k
```

### 2. Refresh Token

```bash
curl -X POST https://localhost/app-api-autenticacao/refresh-token \
     -H "Token: SEU_TOKEN_AQUI" -k
```

---

## 🛠️ Dicas de Teste

1. **Insecure SSL**: Sempre utilize o parâmetro `-k` ao testar contra `https://localhost` para evitar erros de certificado.
2. **Verbose**: Adicione `-v` ao comando para ver os headers detalhados da resposta.
3. **JQ**: Se tiver o `jq` instalado, use `| jq` no final para formatar o JSON retornado.
   *Ex: `curl ... | jq`*
