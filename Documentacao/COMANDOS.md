# 🛠️ Guia de Comandos do Projeto

Este documento centraliza os comandos essenciais para operar o ecossistema **Listas de Compras**.

## 🐙 Comandos Git (Rotina)

Sempre siga este fluxo para manter seu código seguro e sincronizado:

1. **Verificar Alterações**:

   ```bash
   git status
   ```

2. **Adicionar Arquivos**:

   ```bash
   git add .
   ```

3. **Criar Commit**:

   ```bash
   git commit -m "sua mensagem descritiva aqui"
   ```

4. **Enviar para o Servidor**:

   ```bash
   git push
   ```

---

## 🛑 Solução de Problemas (Troubleshooting)

### Erro: `fatal: Unable to create '.git/index.lock': File exists`

Este erro ocorre quando o Git acredita que outro processo está usando o repositório (comum em pastas sincronizadas como o **OneDrive**).

**Como resolver:**

```powershell
del .git/index.lock
```

*Se o erro persistir, feche o VS Code/Visual Studio e tente novamente.*

---

## 🐳 Comandos Docker

### Inicialização Completa

Sobe todos os serviços (Banco, Redis, APIs, Frontends):

```bash
docker-compose up -d --build
```

### Visualizar Logs

Acompanhe o que está acontecendo dentro dos containers:

```bash
docker-compose logs -f
```

### Parar Tudo

```bash
docker-compose down
```

---

## 💻 Desenvolvimento Local (Sem Docker)

Caso queira rodar apenas um componente específico para depuração rápida:

### Backends (.NET)

Navegue até a pasta da `.csproj` correspondente e execute:

```bash
dotnet run
```

### Frontends (Angular/Ionic)

Na pasta raiz do frontend (`Frontend/app-mobile-compras` ou similar):

```bash
npm start
```

---

> [!TIP]
> **Dica de Ouro**: Sempre rode o `git status` antes de começar a trabalhar para garantir que seu ambiente está limpo e atualizado.
