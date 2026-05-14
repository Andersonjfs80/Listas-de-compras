# Core_Logs - Biblioteca de Logs e Utilitários

Esta biblioteca fornece as bases arquiteturais para o ecossistema BMAD, padronizando a comunicação entre Gateway, Backend e Clientes.

---

## 🏗️ Padrão de Resposta (Envelopes)

O ecossistema utiliza um padrão único de resposta composto por duas chaves principais: `data` e `statusProcessamento`.

### 1. BaseCommand (Backend)

Em vez de retornos genéricos, as classes de **Response** devem herdar de `BaseCommand`. Isso permite que o objeto carregue metadados de processamento sem que eles poluam o JSON de dados (via `[JsonIgnore]`).

```csharp
public sealed class OrdemServicoResponse : BaseCommand 
{
    public Guid Id { get; init; }
    public string Titulo { get; init; } = string.Empty;
}
```

### 2. BaseController (Backend)

No Controller, o método `FromCommand` é responsável por extrair os dados e montar o envelope final.

```csharp
[HttpPut("{id}")]
public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarRequest request)
{
    var resultado = await _sender.Send(request);
    return FromCommand(resultado); // Gera { data: {...}, statusProcessamento: {...} }
}
```

### 3. GatewayResponse (Gateway)

Para endpoints manuais no Gateway ou Middlewares, utilize a classe utilitária `GatewayResponse`. Ela possui inteligência para evitar o "double wrapping" (não envelopa o que já veio envelopado do backend).

```csharp
return GatewayResponse.Success(dados);
return GatewayResponse.Error("Acesso Negado", HttpStatusCode.Forbidden);
```

---

---

## 🛡️ Segurança e Criptografia (End-to-End)

A biblioteca agora suporta criptografia de trânsito em duas camadas, permitindo privacidade total sem sacrificar a observabilidade.

### 1. Criptografia de Body (Padrão JOSE/JWE)
Para garantir que os dados não sejam visíveis no Network do navegador, o sistema utiliza **JWE (JSON Web Encryption)** envelopado em JSON.

*   **Algoritmo:** `DIR` (Direct) + `A256GCM` (AES GCM 256 bits).
*   **Padrão de Envelope (Obrigatório):** Para evitar erros de parsing e `415 Unsupported Media Type`, o token JOSE deve ser enviado sempre dentro de um objeto JSON:
    ```json
    { "data": "eyJhbGciOiJkaXIiLCJlbmMiOiJBMjU2R0NNIn0..xxxxx.yyyyy.zzzzz" }
    ```
*   **Funcionamento:** 
    *   **Frontend:** O `EncryptionInterceptor` transforma o JSON no envelope acima.
    *   **Backend:** O `BodyEncryptionMiddleware` (que deve ser o primeiro no pipeline) abre o envelope, descriptografa e restaura o JSON original.
*   **Headers Discretos (Ofuscação):** Para não chamar atenção no Network Tab, não usamos nomes óbvios. O sistema utiliza apenas o header **`X-Sec-Key: 1`** para sinalizar tráfego protegido.

### 2. Criptografia de Campos (SecurityService)
Diferente da criptografia de body, a criptografia de campos é usada para proteção **persistente** ou **pontual** no banco de dados.
*   **Motor:** AES-256-CBC.
*   **Uso:** `_securityService.HashPassword()` ou `EncryptFixed()` para campos sensíveis (Ex: senhas, CPFs).

---

## 📝 Logs e Observabilidade

### 1. Log Batching (Frontend)
Para otimizar a performance, o `LogService` acumula registros e os envia em lote.
*   **Limite de Batch:** 5 logs ou 3 segundos (configurável).
*   **Sanitização:** Logs são capturados **antes** da criptografia do body, permitindo que a sanitização remova senhas (`***`) mas mantenha o restante do JSON legível para análise.

### 2. Kafka Integration
Os logs do backend são disparados de forma assíncrona para o Kafka via `KafkaLoggingMiddleware`.

---

## 🛠️ Instalação (IoC)

Para registrar todos os serviços:
```csharp
builder.Services.AddCoreLogs(builder.Configuration);
// Para ativar a descriptografia automática de body no backend:
app.UseBodyEncryptionMiddleware();
```
