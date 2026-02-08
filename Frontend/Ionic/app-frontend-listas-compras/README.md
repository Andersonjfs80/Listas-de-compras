# App Frontend Listas de Compras (Ionic/Capacitor)

Este é o frontend mobile desenvolvido utilizando Ionic e Capacitor para o sistema de listas de compras.

## 🛠️ Tecnologias e Versões

- **Ionic Framework**: ^8.0.0
- **Angular**: ^20.0.0
- **Capacitor Core/CLI**: ^8.0.0
- **Node.js**: 18+
- **NGINX**: stable-alpine (Docker)

## 🚀 Como Rodar Localmente (Desenvolvimento)

Siga os passos abaixo na ordem de execução:

### 1. Pré-requisitos

Certifique-se de ter o Ionic CLI instalado globalmente:

```bash
npm install -g @ionic/cli
```

### 2. Instalação de Dependências

Navegue até a pasta do projeto e instale os pacotes:

```bash
npm install
```

### 3. Execução do Servidor de Dev

Inicie o servidor para visualização no navegador:

```bash
ionic serve
```

Acesse em: `http://localhost:8100/`

---

## 📱 Desenvolvimento Mobile (Nativo)

### Sincronização com Projetos Nativos

Sempre que alterar o código web, sincronize com os projetos iOS/Android:

```bash
npx cap sync
```

### Abrir nos IDEs Nativos

- **Android**: `npx cap open android` (Requer Android Studio)
- **iOS**: `npx cap open ios` (Requer Xcode)

---

## 🐳 Como Rodar no Docker

### 1. Build e Execução

Utilize o Docker Compose para subir o ambiente simulado com NGINX:

```bash
docker-compose up --build -d
```

Acesse em: `http://localhost:8100` (conforme mapeamento do compose)

---

## 📂 Comandos Úteis

- **Gerar Página**: `ionic generate page pages/nome-pagina`
- **Build Web**: `ionic build`
- **Listar Plugins Capacitor**: `npx cap ls`

---

## 📝 Padrões do Agente

Este projeto segue as diretrizes definidas no arquivo `Padrao_Angular_Frontend_Agente_IA.md`.
