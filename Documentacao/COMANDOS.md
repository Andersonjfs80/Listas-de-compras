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

### 🚀 Subir o Ecossistema

#### Subir tudo (build + detached)

```bash
docker-compose up -d --build
```

#### Subir apenas a infraestrutura (Banco, Redis, Kafka)

```bash
docker-compose -f docker-compose-infra.yml up -d
```

#### Subir um serviço específico (sem rebuikdar os outros)

```bash
docker-compose up -d --build <nome-do-servico>

# Exemplos:
docker-compose up -d --build app-modulo-autenticacao
docker-compose up -d --build app-modulo-home
docker-compose up -d --build app-mobile-compras
docker-compose up -d --build app-backend-autenticacao
docker-compose up -d --build gateway
```

---

### 🛑 Parar Serviços

#### Parar tudo (mantém volumes/dados)

```bash
docker-compose down
```

#### Parar tudo E apagar volumes (⚠️ apaga banco de dados!)

```bash
docker-compose down -v
```

#### Parar um serviço específico

```bash
docker-compose stop <nome-do-servico>
```

#### Reiniciar um serviço específico

```bash
docker-compose restart <nome-do-servico>
```

---

### 📋 Ver Logs

#### Logs de todos os serviços em tempo real

```bash
docker-compose logs -f
```

#### Logs de um serviço específico

```bash
docker-compose logs -f <nome-do-servico>

# Exemplos:
docker-compose logs -f app-modulo-autenticacao
docker-compose logs -f gateway
docker-compose logs -f app-backend-autenticacao
```

#### Últimas N linhas de log

```bash
docker-compose logs --tail=100 <nome-do-servico>
```

---

### 🔍 Inspecionar Containers

#### Ver status de todos os containers

```bash
docker-compose ps
```

#### Entrar dentro de um container (terminal interativo)

```bash
docker exec -it <nome-do-servico> sh

# Para containers .NET (usar bash se disponível):
docker exec -it app-backend-autenticacao bash
```

#### Ver uso de recursos (CPU, memória)

```bash
docker stats
```

---

### 🧹 Limpeza

#### Remover imagens antigas / não utilizadas

```bash
docker image prune -f
```

#### Limpeza completa (imagens, containers parados, redes, cache de build)

```bash
docker system prune -f
```

#### Forçar rebuild sem cache (quando algo "trava" misteriosamente)

```bash
docker-compose build --no-cache <nome-do-servico>
docker-compose up -d <nome-do-servico>
```

---

## 💻 Desenvolvimento Local (Sem Docker)

Caso queira rodar apenas um componente específico para depuração rápida:

### Backends (.NET)

Navegue até a pasta da `.csproj` correspondente e execute:

```bash
dotnet run
```

### Frontends (Angular)

> O projeto usa **pnpm** como gerenciador de pacotes. Instale-o com `npm install -g pnpm`.

Navegue até a pasta do pod desejado e execute:

#### 🔵 Instalar dependências

```bash
pnpm install
```

#### ▶️ Iniciar em desenvolvimento (hot-reload)

```bash
pnpm start
```

#### 🏗️ Build de produção

```bash
# Pod Autenticacao
cd Frontend/Modules/app-modulo-autenticacao
pnpm run build -- --configuration production --base-href /

# Pod Home
cd Frontend/Modules/app-modulo-home
pnpm run build -- --configuration production --base-href /home/

# App Mobile
cd Frontend/app-mobile-compras
pnpm run build -- --configuration production
```

#### ➕ Adicionar um novo pacote

```bash
pnpm add <nome-do-pacote>

# Somente em devDependencies:
pnpm add -D <nome-do-pacote>
```

#### 🗑️ Remover um pacote

```bash
pnpm remove <nome-do-pacote>
```

#### 🔄 Atualizar pacotes

```bash
pnpm update
```

---

> [!TIP]
> **Dica de Ouro**: Sempre rode o `git status` antes de começar a trabalhar para garantir que seu ambiente está limpo e atualizado.
