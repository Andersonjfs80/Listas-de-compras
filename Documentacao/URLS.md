# 🌐 Guia de URLs e Endpoints

Este documento lista todas as URLs de acesso para os ambientes de **Desenvolvimento Local** (rodando as APIs direto no Windows/IDE) e **Homologação** (Ecossistema Docker completo).

## 🚀 Ambiente Homologação (Docker)

Neste ambiente, o **Nginx Gateway** centraliza os acessos na porta padrão HTTPS (443).

### 🖥️ Frontends (Interface do Usuário)

| Componente | URL de Acesso | Descrição |
| :--- | :--- | :--- |
| **Ponto de Entrada** | [https://localhost](https://localhost) | Redireciona para o módulo Home. |
| **Módulo Home** | [https://localhost/home](https://localhost/home) | Dashboard principal e listagens. |
| **Autenticação** | [https://localhost/autenticacao](https://localhost/autenticacao) | Tela de Login e Registro. |
| **App Shell** | [https://localhost/shell](https://localhost/shell) | Container principal das apps. |

### 🛠️ Gateways e APIs (Via Nginx)

| API | URL Base | Swagger / Docs |
| :--- | :--- | :--- |
| **API Produto** | `https://localhost/app-api-produto` | [Swagger](https://localhost/app-api-produto/swagger) |
| **API Autenticação** | `https://localhost/app-api-autenticacao` | [Swagger](https://localhost/app-api-autenticacao/swagger) |

### 🔍 Acesso Direto aos Backends (Swagger/Debug)

Útil para testar os backends sem passar pelo Gateway:

- **Backend Produto**: [https://localhost:6002/swagger](https://localhost:6002/swagger)
- **Backend Autenticação**: [https://localhost:7000/swagger](https://localhost:7000/swagger)
- **Backend Notificação**: [https://localhost:7008/swagger](https://localhost:7008/swagger)

---

## 💻 Ambiente Desenvolvimento Local (IDE/Windows)

Se você estiver rodando os projetos direto pelo Visual Studio ou `dotnet run` no Windows:

| Serviço | Porta HTTP | Porta HTTPS |
| :--- | :--- | :--- |
| **API Autenticacao (Gateway)** | 5006 | 5005 |
| **API Produto (Gateway)** | 5022 | 5021 |
| **Backend Autenticacao** | 7001 | 7000 |
| **Backend Produto** | 6001 | 6002 |
| **Backend Notificação** | 7009 | 7008 |

---

## 🗄️ Infraestrutura e Ferramentas

| Serviço | Porta | Ferramenta Recomendada |
| :--- | :--- | :--- |
| **SQL Server** | 1433 | SQL Server Management Studio (SSMS) / Azure Data Studio |
| **Redis** | 6379 | Redis Insight |
| **Kafka** | 9092 | Offset Explorer (Kafka Tool) |
| **Kafka UI** | 8080 | [http://localhost:8080](http://localhost:8080) (Se configurado no Compose) |

---

> [!IMPORTANT]
> **Certificados HTTPS**: Ao acessar as URLs `https`, o navegador mostrará um aviso de "Conexão não segura" devido aos certificados autoassinados. Clique em **Avançado -> Prosseguir para localhost**.
