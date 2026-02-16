# 🔒 Guia de Configuração HTTPS

Este guia descreve como o ambiente foi configurado para utilizar HTTPS em todos os serviços (Frontend, Backend e Gateway) utilizando o Nginx como proxy reverso e certificados autoassinados para desenvolvimento.

## 📋 Pré-requisitos

- **Docker** e **Docker Compose** instalados.
- **OpenSSL** (geralmente instalado com Git Bash no Windows ou nativo no Linux/Mac).

## 🛠️ Passo a Passo da Configuração

### 1. Geração de Certificados

Para habilitar o HTTPS, geramos certificados autoassinados. Os arquivos ficam localizados em `./nginx/certs`.

**Comandos utilizados para gerar os certificados:**

1. **Gerar Chave Privada e Certificado Público (CRT/KEY):**

    ```bash
    openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout nginx/certs/localhost.key -out nginx/certs/localhost.crt -subj "/C=BR/ST=SP/L=Sao Paulo/O=Dev/CN=localhost"
    ```

2. **Gerar Certificado PFX (para .NET Backends):**
    O .NET exige um certificado no formato `.pfx` (PKCS#12) com senha.

    ```bash
    openssl pkcs12 -export -out nginx/certs/localhost.pfx -inkey nginx/certs/localhost.key -in nginx/certs/localhost.crt -passout pass:Development
    ```

    *Senha utilizada: `Development`*

### 2. Configuração do Nginx Gateway

O arquivo `nginx/gateway.conf` foi configurado para:

- Escutar na porta **443** (HTTPS) com SSL habilitado.
- Utilizar os certificados `localhost.crt` e `localhost.key`.
- Redirecionar o tráfego para os serviços de backend (Autenticação, Produto, etc.) via protocolo `https://`.
- Ignorar a verificação de certificado SSL dos backends (`proxy_ssl_verify off`), pois eles também usam certificados autoassinados.

### 3. Configuração dos Backends (.NET)

Os serviços de backend (`app-backend-autenticacao`, `app-backend-produto`, etc.) foram configurados no `docker-compose.yml`:

- **Porta Interna**: Alterada para **8081** (padrão HTTPS configurado).
- **Variáveis de Ambiente**:
  - `ASPNETCORE_URLS=https://+:8081`: Define a URL e porta segura.
  - `ASPNETCORE_Kestrel__Certificates__Default__Path=/https/localhost.pfx`: Caminho do certificado dentro do container.
  - `ASPNETCORE_Kestrel__Certificates__Default__Password=Development`: Senha do certificado.
- **Volumes**: O arquivo `localhost.pfx` é montado em `/https/localhost.pfx` dentro dos containers.

### 4. Configuração dos Frontends

Os frontends (Angular/Ionic) foram atualizados para apontar para as URLs seguras:

- **API URLs**: Alteradas de `http://` para `https://`.
- **Iframe Url**: No app mobile, o wrapper do home agora carrega via HTTPS.

## 🚀 Como Executar

Para subir o ambiente com todas as configurações HTTPS aplicadas:

1. Certifique-se de que os certificados existam na pasta `nginx/certs`.
2. Execute o comando de build e subida dos containers:

    ```bash
    docker-compose up -d --build
    ```

## ✅ Verificação

Após subir os containers:

1. Acesse **[https://localhost](https://localhost)** no navegador.
    - Você verá um alerta de segurança ("Sua conexão não é particular"). Isso é normal para certificados autoassinados.
    - Clique em **Avançado** -> **Ir para localhost (não seguro)**.
2. O Frontend deve carregar corretamente e conseguir se comunicar com as APIs.
3. Você pode acessar as APIs diretamente (ex: `https://localhost:7000/health`), aceitando o risco de segurança também.

---
**Nota**: Em produção, você deve substituir os certificados autoassinados por certificados válidos emitidos por uma autoridade certificadora (CA).
