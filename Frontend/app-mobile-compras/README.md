# App Mobile Compras - Shell Micro-Frontend

Este é o **shell** do sistema de listas de compras, seguindo arquitetura **micro-frontend**. As funcionalidades principais são carregadas do **app-modulo-home**.

## 🏗️ Arquitetura

```text
app-mobile-compras (Shell - 4200)
├── Welcome Page       - Tela inicial
├── Toolbar            - Barra superior
└── BottomNav          - Navegação inferior
    ↓ redireciona para
app-modulo-home (Módulo - 4202)
├── Home               - Dashboard
├── Listas             - Gerenciamento de listas
├── Ofertas            - Produtos em promoção
└── Carrinho           - Carrinho de compras
```

## 🛠️ Tecnologias

- **Angular**: ^21.1.3
- **Angular Material**: ^21.1.3  
- **Node.js**: 18+
- **NGINX**: stable-alpine (Docker)

## 🚀 Como Rodar

### Desenvolvimento

```bash
npm install
ng serve
```

Acesse: `http://localhost:4200/`  
*(A Welcome page redirecionará para `localhost:4202`)*

### Docker

```bash
docker-compose up -d --build
```

Acesse: `http://localhost:8080`

---

## 📂 Estrutura do Projeto

```text
src/app/
├── layout/            # Componentes do shell
│   ├── toolbar/       # Barra superior
│   └── bottom-nav/    # Navegação inferior
└── features/
    └── welcome/       # Página inicial (redireciona para módulo)
```

## 🎯 Clean Code

**Zero duplicação de código!**

- ✅ Shell contém apenas estrutura
- ✅ Funcionalidades no app-modulo-home
- ✅ Separação clara de responsabilidades

## 📝 Comandos Úteis

- **Dev Server**: `ng serve`
- **Build**: `ng build`
- **Docker Up**: `docker-compose up -d`
- **Docker Down**: `docker-compose down`

---

## 🔄 Integração Futura

Para integrar os módulos de forma nativa (sem redirecionamento), considere:

- **Webpack Module Federation**
- **Single-SPA Framework**
- **NGINX Proxy Reverso**
