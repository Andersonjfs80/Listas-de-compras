# Build com contexto na raiz do repo (necessário para @app/logs -> Frontend/Library)
FROM node:22-alpine AS build-step
RUN npm install -g pnpm && npm install -g @angular/cli
WORKDIR /workspace

# Copiar Library (path ../../Library no tsconfig)
COPY Frontend/Library ./Frontend/Library/

# Instalar dependências na raiz
WORKDIR /workspace
COPY Frontend/Modules/app-modulo-home/package.json ./
COPY Frontend/Modules/app-modulo-home/pnpm-lock.yaml ./
RUN pnpm install

# Copiar o resto do app
WORKDIR /workspace/Frontend/Modules/app-modulo-home
COPY Frontend/Modules/app-modulo-home/ ./

# Build da aplicação
ARG BUILD_CONFIG=production
RUN pnpm exec ng build --configuration ${BUILD_CONFIG} --base-href /home/

# Estágio 2: Serve com Angular SSR (Node.js)
FROM node:22-alpine
WORKDIR /app
COPY --from=build-step /workspace/Frontend/Modules/app-modulo-home/dist/app-modulo-home ./dist/app-modulo-home
ENV PORT=4000
EXPOSE 4000
CMD sh -c 'if [ "$DEBUG_MODE" = "true" ]; then exec node --inspect=0.0.0.0:${DEBUG_PORT:-9229} dist/app-modulo-home/server/server.mjs; else exec node dist/app-modulo-home/server/server.mjs; fi'
