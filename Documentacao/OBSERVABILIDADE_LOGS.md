# 📊 Documentação de Observabilidade e Logs (Stack ELK + Kafka)

Este documento descreve a implementação do pipeline de logs do projeto **Listas de Compras**, cobrindo desde a captura no Backend até a visualização no Kibana.

---

## 🏗️ Arquitetura do Pipeline

O fluxo de logs segue o padrão de alta performance e desacoplamento:

1.  **Backend/Gateway**: Captura o evento (Request/Response) e envia para o Kafka.
2.  **Kafka**: Atua como um buffer (esteira de mensagens), garantindo que o Backend não trave se o Elastic estiver lento.
3.  **Logstash**: Atua como o "trabalhador" que consome do Kafka, transforma os dados e os entrega ao Elasticsearch.
4.  **Elasticsearch**: Banco de dados de busca onde os logs são indexados.
5.  **Kibana**: Interface visual para consulta e criação de dashboards.

---

## 🛠️ Tecnologias Utilizadas

- **Kafka & Zookeeper**: Mensageria distribuída.
- **Logstash 8.12**: ETL (Extract, Transform, Load) para logs.
- **Elasticsearch 8.12**: Motor de busca e análise.
- **Kibana 8.12**: Painel de controle visual.
- **.NET Core Library (Core_Logs)**: Biblioteca customizada para interceptação de tráfego HTTP.

---

## 🛡️ Segurança e Privacidade (Data Masking)

Implementamos uma camada de **Segurança por Padrão (Secure by Default)** na biblioteca `Core_Logs`:
- **JsonSanitizer**: Um utilitário que usa Regex para detectar e ofuscar campos sensíveis.
- **Campos Ofuscados automaticamente**: `senha`, `password`, `token`, `secret`, `key`, `senhaAcesso`.
- Mesmo que o desenvolvedor esqueça de configurar, a biblioteca protege esses dados antes de saírem da aplicação.

---

## 🚀 Como os Logs são Gerados

Configuramos o `KafkaLoggingMiddleware` para gerar **dois eventos distintos** por chamada HTTP, garantindo rastreabilidade total:

### 1. Evento de Request (`Tipo: request`)
- **Body**: Contém o corpo enviado pelo cliente.
- **Enriquecimento**: Contém `Path`, `FullUrl`, `Method`, `Headers` e `Timestamp`.
- Enviado assim que a requisição chega no servidor.

### 2. Evento de Response (`Tipo: response`)
- **Response**: Contém o retorno da API (devidamente ofuscado).
- **Métricas**: Contém o `StatusCode` e `DurationMs` (tempo de processamento).
- Enviado após a conclusão do processamento.

---

## ⚙️ Passo a Passo de Configuração

### 1. Infraestrutura (Docker)
O Logstash é configurado via `docker-compose-infra.yml`. Utilizamos a técnica de injeção de configuração via `command` para evitar problemas de permissão de arquivos no Windows:
```yaml
command: 
  - |
    echo "input { kafka { ... } } output { elasticsearch { ... } }" > /usr/share/logstash/pipeline/logstash.conf
    /usr/share/logstash/bin/logstash
```

### 2. Visualização no Kibana
Para ver os logs, siga estes passos:
1. Acesse `http://localhost:5601`.
2. Vá em **Stack Management** -> **Data Views**.
3. Crie uma Data View com o nome `listas-compras-logs-*`.
4. Use o campo `@timestamp` para a linha do tempo.
5. No **Discover**, adicione as colunas `Tipo`, `Method`, `Path`, `Body` e `Response` para uma visão tabular perfeita.

---

## 🔍 Dicas de Análise
- **Filtrar por erro**: Adicione um filtro `StatusCode > 299`.
- **Analisar performance**: Ordene pela coluna `DurationMs` de forma decrescente.
- **Rastrear caminho**: Use o campo `FullUrl` para entender por qual Gateway a requisição passou.

---
*Documentação gerada em 06/05/2026 pelo Antigravity AI.*
