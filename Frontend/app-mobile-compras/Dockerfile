# Build com contexto na raiz do repo (necessário para @app/logs -> Frontend/Library)
FROM node:22-alpine AS build-step
RUN npm install -g pnpm && npm install -g @angular/cli
WORKDIR /workspace

# Copiar Library (path ../Library no tsconfig)
COPY Frontend/Library ./Frontend/Library/

# Instalar dependências na raiz para que tanto App quanto Library enxerguem
WORKDIR /workspace
COPY Frontend/app-mobile-compras/package.json ./
COPY Frontend/app-mobile-compras/pnpm-lock.yaml ./
RUN pnpm install

# Copiar o resto do app
WORKDIR /workspace/Frontend/app-mobile-compras
COPY Frontend/app-mobile-compras/ ./

# Build da aplicação
ARG BUILD_CONFIG=production
RUN pnpm exec ng build --configuration ${BUILD_CONFIG}

# Estágio 2: Serve com Angular SSR (Node.js)
FROM node:22-alpine
WORKDIR /app
COPY --from=build-step /workspace/Frontend/app-mobile-compras/dist/app-mobile-compras ./dist/app-mobile-compras
ENV PORT=4000
EXPOSE 4000
CMD sh -c 'if [ "$DEBUG_MODE" = "true" ]; then exec node --inspect=0.0.0.0:${DEBUG_PORT:-9231} dist/app-mobile-compras/server/server.mjs; else exec node dist/app-mobile-compras/server/server.mjs; fi'
