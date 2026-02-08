# App Frontend Listas de Compras (Angular)

Este é o frontend web desenvolvido em Angular seguindo a estrutura de mercado para o sistema de listas de compras.

## 🛠️ Tecnologias e Versões

- **Angular**: ^21.1.3
- **Angular Material**: ^21.1.3
- **Node.js**: 18+ (Recomendado)
- **NPM**: 10+
- **NGINX**: stable-alpine (Docker)

## 🚀 Como Rodar Localmente (Desenvolvimento)

Siga os passos abaixo na ordem de execução:

### 1. Pré-requisitos

Certifique-se de ter o Node.js e o Angular CLI instalados globalmente:

```bash
npm install -g @angular/cli
```

### 2. Instalação de Dependências

Navegue até a pasta do projeto e instale os pacotes:

```bash
npm install
```

### 3. Execução do Servidor de Dev

Inicie o servidor local:

```bash
ng serve
```

Acesse em: `http://localhost:4200/`

---

## 🐳 Como Rodar no Docker (Produção/Híbrido)

Este projeto está configurado para simular o ambiente OpenShift usando NGINX.

### 1. Build da Imagem

Cria a imagem Docker localmente:

```bash
docker build -t app-listas-compras-angular .
```

### 2. Execução com Docker Compose

Sobe o container com as configurações de rede e porta:

```bash
docker-compose up -d
```

Acesse em: `http://localhost:8080`

---

## 📂 Comandos Úteis

- **Gerar Componente**: `ng generate component features/nome-componente`
- **Build de Produção**: `ng build`
- **Executar Testes**: `ng test`
- **Parar Docker**: `docker-compose down`

---

## 📝 Padrões do Agente

Este projeto segue as diretrizes definidas no arquivo `Padrao_Angular_Frontend_Agente_IA.md`.
