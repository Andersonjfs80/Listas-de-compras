# Build com contexto na raiz do repo
FROM node:22-alpine AS build-step
RUN npm install -g npm@11
WORKDIR /workspace

# Copiar Library
COPY Frontend/Library ./Frontend/Library/

# Instalar dependências na raiz para que tanto App quanto Library enxerguem
WORKDIR /workspace
COPY Frontend/Modules/app-modulo-autenticacao/package.json ./
RUN npm install

# Copiar a Library e o App (depois do npm install para garantir contexto)
WORKDIR /workspace/Frontend/Library/app-library-logs
COPY Frontend/Library/app-library-logs/ ./
WORKDIR /workspace/Frontend/Modules/app-modulo-autenticacao
COPY Frontend/Modules/app-modulo-autenticacao/ ./

RUN npm run build -- --configuration production --base-href /autenticacao/

# Estágio 2: Serve
FROM nginx:stable-alpine
COPY --from=build-step /workspace/Frontend/Modules/app-modulo-autenticacao/dist/app-modulo-autenticacao/browser /usr/share/nginx/html
# Criar um arquivo nginx básico
RUN echo 'server { \
    listen 80; \
    location / { \
    root /usr/share/nginx/html; \
    index index.html index.htm; \
    try_files $uri $uri/ /index.html; \
    } \
    }' > /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
