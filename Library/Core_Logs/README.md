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

## 🛡️ Segurança e Tokens

A biblioteca fornece uma abstração para Tokens, permitindo alternar entre transparência (JWT) e privacidade total (JOSE/Criptografado).

### Configuração (appsettings.json)

```json
"Security": {
  "TokenProvider": "JOSE", // Opções: JWT ou JOSE
  "SecretKey": "sua-chave-ultra-secreta-de-32-chars"
}
```

### ITokenService

Injete `ITokenService` para manipular sessões de usuário de forma agnóstica ao provedor.

```csharp
public class LoginHandler(ITokenService tokenService)
{
    public string Handle() 
    {
        var session = new UserSession { Id = "123", NomeExibicao = "Admin", Documento = "12345678900" };
        return tokenService.GenerateToken(session);
    }
}
```

---

## 🌐 Gateway e Headers Padronizados

O sistema exige e propaga automaticamente um conjunto de headers obrigatórios definidos em `StandardHeaderNames`:

* `TOKEN`: Token de autenticação.
* `SIGLA-APLICACAO`: Identificador do sistema de origem.
* `SESSAO-ID`: ID único da sessão do usuário.
* `MESSAGE-ID`: ID único da transação/mensagem.

### Ativação Automática (Gateway)

No `Program.cs` do Gateway, você ativa a segurança e o envelopamento global em apenas duas linhas:

```csharp
app.UseHeaderValidation(); // Valida headers obrigatórios

var apiGroup = app.MapGroup("/api")
                  .AddGatewayAutoEnvelope(); // Envelopa tudo automaticamente
```

---

## 📝 Logs e Kafka

Integração nativa para envio de logs assíncronos para o Kafka.

### Configuração

```json
"KafkaSettings": {
  "BootstrapServers": "localhost:9092",
  "Topic": "system-logs"
}
```

### Uso do Logger

```csharp
public class MeuServico(IKafkaLogger logger)
{
    public void FazerAlgo() => logger.LogAsync("Ação realizada com sucesso");
}
```

---

## 🛠️ Instalação (IoC)

Para registrar todos os serviços (Segurança, Logs, Configurações), utilize o método de extensão:

```csharp
builder.Services.AddCoreLogs(builder.Configuration);
```
