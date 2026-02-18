# Estágio 1: Build
FROM node:22-alpine AS build-step
RUN npm install -g npm@11
WORKDIR /app

# Copiar package.json e package-lock.json
COPY Frontend/app-mobile-compras/package*.json ./

# Instalar dependências
RUN npm install

# Copiar o código fonte
COPY Frontend/app-mobile-compras/ ./

# Build da aplicação
RUN npm run build -- --configuration production

# Estágio 2: Serve com NGINX
FROM nginx:stable-alpine

# Copiar os arquivos buildados para o NGINX
COPY --from=build-step /app/dist/app-mobile-compras/browser /usr/share/nginx/html

# Copiar configuração do NGINX
COPY Frontend/app-mobile-compras/nginx/nginx.conf /etc/nginx/conf.d/default.conf

# Copiar certificados
COPY certificados/localhost.crt /etc/nginx/certs/localhost.crt
COPY certificados/localhost.key /etc/nginx/certs/localhost.key

EXPOSE 80 443
CMD ["nginx", "-g", "daemon off;"]
