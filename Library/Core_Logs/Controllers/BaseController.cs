using Microsoft.AspNetCore.Mvc;
using Core_Logs.Commands;
using Core_Logs.Models.Request;
using System.Security.Claims;

namespace Core_Logs.Controllers;

/// <summary>
/// Controller base que padroniza retornos da API usando BaseCommand.
/// Suporta automaticamente status codes: 200, 250 (notificação), 422 (regra), 550 (erro notificação).
/// </summary>
[ApiController]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Retorna o e-mail do usuário autenticado extraído do Token.
    /// </summary>
    protected string? EmailUsuarioLogado => User.FindFirstValue(ClaimTypes.Email);
    /// <summary>
    /// Converte um BaseCommand em IActionResult, respeitando automaticamente o status code.
    /// Este é o método PRINCIPAL e RECOMENDADO para retornar dados dos handlers.
    /// </summary>
    /// <remarks>
    /// Status codes suportados automaticamente:
    /// - 200: Sucesso normal
    /// - 250: Sucesso de notificação
    /// - 422: Erro de regra de negócio
    /// - 550: Erro de notificação
    /// - Outros: Conforme definido no BaseCommand
    /// 
    /// Retorno sempre inclui:
    /// - data: Dados do retorno (se houver)
    /// - statusProcessamento: Informações sobre o processamento
    /// 
    /// Lógica de notificações:
    /// - Se houver APENAS 1 notificação: incorpora ao statusProcessamento
    /// - Se houver MAIS DE 1: retorna array separado (sucessos/warnings/erros)
    /// </remarks>
    protected IActionResult FromCommand(BaseCommand command)
    {
        var response = new Dictionary<string, object?>
        {
            { "data", command }
        };

        // Por padrão, usa dados do command
        string codigoProcessamento = command.CodigoProcessamento ?? (command.Sucesso ? "SUCCESS" : "ERROR");
        string? mensagemProcessamento = command.MensagemProcessamento ?? command.Mensagem;

        // Conta total de notificações (apenas warnings e erros)
        int totalNotificacoes = command.Warnings.Count + command.Erros.Count;        

        // Se houver apenas 1 warning ou erro, sobrescreve com seus dados
        if (totalNotificacoes == 1)
        {
            var notificacao = command.Warnings.FirstOrDefault() ?? command.Erros.FirstOrDefault();

            if (notificacao != null)
            {
                codigoProcessamento = notificacao.Codigo;
                mensagemProcessamento = notificacao.Mensagem;
            }
        }

        // Cria statusProcessamento (sempre existe)
        var statusProcessamento = new
        {
            codigoProcessamento,
            mensagemProcessamento,
            detalhesProcessamento = command.Mensagem
        };

        response.Add("statusProcessamento", statusProcessamento);

        // Adiciona arrays de notificações apenas se houver mais de 1
        if (totalNotificacoes > 1)
        {
            if (command.Sucessos.Any())
                response.Add("sucessos", command.Sucessos);

            if (command.Warnings.Any())
                response.Add("warnings", command.Warnings);

            if (command.Erros.Any())
                response.Add("erros", command.Erros);
        }

        return StatusCode((int)command.Status, response);
    }

    // ========== MÉTODOS AUXILIARES (Para casos sem BaseCommand) ==========

    /// <summary>
    /// Retorna 200 OK com dados encapsulados em { data: {...} }
    /// Use FromCommand quando possível para respostas mais ricas.
    /// </summary>
    protected IActionResult OkData<T>(T data)
    {
        return Ok(new { data });
    }

    /// <summary>
    /// Retorna 201 Created com dados encapsulados em { data: {...} }
    /// Use FromCommand quando possível para respostas mais ricas.
    /// </summary>
    protected IActionResult CreatedData<T>(string actionName, object routeValues, T data)
    {
        return CreatedAtAction(actionName, routeValues, new { data });
    }

    /// <summary>
    /// Retorna 204 No Content (para DELETE bem-sucedido)
    /// </summary>
    protected new IActionResult NoContent()
    {
        return base.NoContent();
    }

    // ========== EXTRAÇÃO DE HEADERS PADRONIZADOS ==========

    /// <summary>
    /// Extrai os headers padronizados da requisição HTTP
    /// </summary>
    /// <returns>Objeto RequestHeaders com os valores dos headers</returns>
    protected RequestHeaders ObterHeaders()
    {
        var headers = new RequestHeaders();

        // Extrai SIGLA-APLICACAO
        if (Request.Headers.TryGetValue("SIGLA-APLICACAO", out var siglaAplicacao))
        {
            headers.SiglaAplicacao = siglaAplicacao.ToString();
        }

        // Extrai SESSAO-ID (ou gera um novo se não existir)
        if (Request.Headers.TryGetValue("SESSAO-ID", out var sessionId))
        {
            headers.SessionId = sessionId.ToString();
        }
        else
        {
            headers.SessionId = Guid.NewGuid().ToString();
        }

        // Extrai MESSAGE-ID (ou gera um novo se não existir)
        if (Request.Headers.TryGetValue("MESSAGE-ID", out var messageId))
        {
            headers.MessageId = messageId.ToString();
        }
        else
        {
            headers.MessageId = Guid.NewGuid().ToString();
        }

        // Extrai Authorization
        if (Request.Headers.TryGetValue("Authorization", out var authorization))
        {
            headers.Authorization = authorization.ToString();
            
            // Extrai o token removendo o prefixo "Bearer "
            var authValue = authorization.ToString();
            if (authValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                headers.Token = authValue.Substring(7);
            }
            else
            {
                headers.Token = authValue;
            }
        }

        // Extrai DISPOSITIVO-ID
        if (Request.Headers.TryGetValue("DISPOSITIVO-ID", out var dispositivoId))
        {
            headers.DispositivoId = dispositivoId.ToString();
        }

        return headers;
    }
}
