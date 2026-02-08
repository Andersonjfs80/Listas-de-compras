using System.Net;
using System.Text.Json.Serialization;

namespace Core_Logs.Commands;

/// <summary>
/// Classe base para respostas de comandos/handlers.
/// Use herança nas suas classes de Response para ganhar as funcionalidades de status e notificações.
/// </summary>
public abstract class BaseCommand
{
    /// <summary>
    /// Status HTTP da operação.
    /// Marcado com JsonIgnore para não aparecer duplicado dentro do objeto 'data'.
    /// </summary>
    [JsonIgnore]
    public HttpStatusCode Status { get; set; } = HttpStatusCode.OK;

    /// <summary>
    /// Mensagem principal da operação.
    /// Marcado com JsonIgnore para ser movido para 'statusProcessamento'.
    /// </summary>
    [JsonIgnore]
    public string Mensagem { get; set; } = string.Empty;

    /// <summary>
    /// Código de processamento/erro.
    /// </summary>
    [JsonIgnore]
    public string? CodigoProcessamento { get; set; }

    /// <summary>
    /// Mensagem detalhada de processamento (opcional).
    /// </summary>
    [JsonIgnore]
    public string? MensagemProcessamento { get; set; }

    /// <summary>
    /// Lista de notificações de sucesso.
    /// </summary>
    [JsonIgnore]
    public List<Notificacao> Sucessos { get; set; } = new();

    /// <summary>
    /// Lista de notificações de warning.
    /// </summary>
    [JsonIgnore]
    public List<Notificacao> Warnings { get; set; } = new();

    /// <summary>
    /// Lista de notificações de erro.
    /// </summary>
    [JsonIgnore]
    public List<Notificacao> Erros { get; set; } = new();

    /// <summary>
    /// Indica se a operação foi bem-sucedida.
    /// </summary>
    [JsonIgnore]
    public bool Sucesso => (int)Status >= 200 && (int)Status < 300;

    /// <summary>
    /// Indica se há erros.
    /// </summary>
    [JsonIgnore]
    public bool TemErros => Erros.Any();

    /// <summary>
    /// Indica se há warnings.
    /// </summary>
    [JsonIgnore]
    public bool TemWarnings => Warnings.Any();

    // ========== MÉTODOS DE AUXÍLIO (Fluentes) ==========

    public BaseCommand ComStatus(HttpStatusCode status) { Status = status; return this; }
    public BaseCommand ComMensagem(string mensagem) { Mensagem = mensagem; return this; }
    public BaseCommand ComCodigo(string codigo) { CodigoProcessamento = codigo; return this; }

    public BaseCommand AdicionarErro(string codigo, string mensagem)
    {
        Erros.Add(new Notificacao { Codigo = codigo, Mensagem = mensagem, Tipo = TipoNotificacao.Erro });
        if (Status == HttpStatusCode.OK) Status = HttpStatusCode.BadRequest;
        return this;
    }

    public BaseCommand AdicionarWarning(string codigo, string mensagem)
    {
        Warnings.Add(new Notificacao { Codigo = codigo, Mensagem = mensagem, Tipo = TipoNotificacao.Warning });
        return this;
    }
}

/// <summary>
/// Representa uma notificação individual
/// </summary>
public class Notificacao
{
    public string Codigo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public TipoNotificacao Tipo { get; set; }
    public DateTime DataHora { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Tipos de notificação
/// </summary>
public enum TipoNotificacao
{
    Sucesso,
    Warning,
    Erro
}
