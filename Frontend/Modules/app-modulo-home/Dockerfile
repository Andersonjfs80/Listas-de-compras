# Build com contexto na raiz do repo (necessário para @app/logs -> Frontend/Library)
FROM node:22-alpine AS build-step
RUN npm install -g npm@11
WORKDIR /workspace

# Copiar Library (path ../../Library no tsconfig)
COPY Frontend/Library ./Frontend/Library/

# Instalar dependências na raiz 
WORKDIR /workspace
COPY Frontend/Modules/app-modulo-home/package.json ./
RUN npm install

# Copiar a Library e o App (depois do npm install para garantir contexto)
WORKDIR /workspace/Frontend/Library/app-library-logs
COPY Frontend/Library/app-library-logs/ ./
WORKDIR /workspace/Frontend/Modules/app-modulo-home
COPY Frontend/Modules/app-modulo-home/ ./
# Copiar o resto do app
COPY Frontend/Modules/app-modulo-home/ ./
RUN npm run build -- --configuration production --base-href /home/

# Serve
FROM nginx:stable-alpine
COPY --from=build-step /workspace/Frontend/Modules/app-modulo-home/dist/app-modulo-home/browser /usr/share/nginx/html
COPY certificados/localhost.crt /etc/nginx/certs/localhost.crt
COPY certificados/localhost.key /etc/nginx/certs/localhost.key

RUN echo 'server { \
    listen 80; \
    listen 443 ssl; \
    ssl_certificate /etc/nginx/certs/localhost.crt; \
    ssl_certificate_key /etc/nginx/certs/localhost.key; \
    location / { \
    root /usr/share/nginx/html; \
    index index.html index.htm; \
    try_files $uri $uri/ /index.html; \
    } \
    }' > /etc/nginx/conf.d/default.conf
EXPOSE 80 443
CMD ["nginx", "-g", "daemon off;"]
