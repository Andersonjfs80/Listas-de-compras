using MediatR;
using app_backend_autenticacao.domain.Commands.Autenticacao.Requests;
using app_backend_autenticacao.domain.Commands.Autenticacao.Responses;
using app_backend_autenticacao.domain.Interfaces.Repositories;
using Core_Logs.Interfaces;
using System.Net;

namespace app_backend_autenticacao.domain.Handlers.Autenticacao;

public class ResetarSenhaHandler(
    IUsuarioRepository repository,
    ICacheService cacheService) : IRequestHandler<ResetarSenhaRequest, ResetarSenhaResponse>
{
    private readonly IUsuarioRepository _repository = repository;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<ResetarSenhaResponse> Handle(ResetarSenhaRequest request, CancellationToken cancellationToken)
    {
        var response = new ResetarSenhaResponse();

        // 1. Buscar usuário
        var usuario = await _repository.ObterPorEmailAsync(request.Email, cancellationToken);
        
        // 2. Validação de existência
        if (usuario == null)
        {
             return (ResetarSenhaResponse)response
                .AdicionarErro("AUTH005", "O e-mail informado não foi encontrado em nossa base.")
                .ComStatus(System.Net.HttpStatusCode.NotFound);
        }

        // 3. Gerar código de recuperação (6 dígitos)
        var random = new Random();
        var codigo = random.Next(100000, 999999).ToString();
        
        // 4. Salvar no Cache (Válido por 15 minutos)
        var cacheKey = $"Auth:ResetCode:{request.Email}";
        await _cacheService.SetAsync(cacheKey, codigo, TimeSpan.FromMinutes(15), cancellationToken);

        // 5. Simular envio de e-mail (Log de Auditoria)
        // Em produção, aqui chamaria um IEmailService.
        // Por enquanto, o código é retornado na mensagem para facilitar o desenvolvimento/teste
        return (ResetarSenhaResponse)response
            .ComMensagem($"Um código de recuperação foi enviado para o seu e-mail. (DEBUG: {codigo})")
            .ComStatus(HttpStatusCode.OK);
    }
}

