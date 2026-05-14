# Build com contexto na raiz do repo
FROM node:22-alpine AS build-step
RUN npm install -g pnpm && npm install -g @angular/cli
WORKDIR /workspace

# Copiar Library
COPY Frontend/Library ./Frontend/Library/

# Instalar dependências na raiz para que tanto App quanto Library enxerguem
WORKDIR /workspace
COPY Frontend/Modules/app-modulo-autenticacao/package.json ./
COPY Frontend/Modules/app-modulo-autenticacao/pnpm-lock.yaml ./
RUN pnpm install

# Copiar o código fonte do App
WORKDIR /workspace/Frontend/Modules/app-modulo-autenticacao
COPY Frontend/Modules/app-modulo-autenticacao/ ./

# Build da aplicação
ARG BUILD_CONFIG=production
RUN pnpm exec ng build --configuration ${BUILD_CONFIG} --base-href /

# Estágio 2: Serve com Angular SSR (Node.js)
FROM node:22-alpine
WORKDIR /app
COPY --from=build-step /workspace/Frontend/Modules/app-modulo-autenticacao/dist/app-modulo-autenticacao ./dist/app-modulo-autenticacao
ENV PORT=4000
EXPOSE 4000
CMD sh -c 'if [ "$DEBUG_MODE" = "true" ]; then exec node --inspect=0.0.0.0:${DEBUG_PORT:-9230} dist/app-modulo-autenticacao/server/server.mjs; else exec node dist/app-modulo-autenticacao/server/server.mjs; fi'
