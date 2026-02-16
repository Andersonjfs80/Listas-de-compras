using MediatR;
using app_backend_autenticacao.domain.Commands.Autenticacao.Requests;
using app_backend_autenticacao.domain.Commands.Autenticacao.Responses;
using app_backend_autenticacao.domain.Interfaces.Repositories;
using app_backend_autenticacao.domain.Models;
using System.Net;

namespace app_backend_autenticacao.domain.Handlers.Autenticacao;

public class CadastrarUsuarioHandler(
    IUsuarioRepository repository) : IRequestHandler<CadastrarUsuarioRequest, CadastrarUsuarioResponse>
{
    private readonly IUsuarioRepository _repository = repository;

    public async Task<CadastrarUsuarioResponse> Handle(CadastrarUsuarioRequest request, CancellationToken cancellationToken)
    {
        var response = new CadastrarUsuarioResponse();

        // 1. Validar se e-mail, documento ou apelido já existem
        if (await _repository.ExisteEmailAsync(request.Email, cancellationToken))
        {
            return (CadastrarUsuarioResponse)response.AdicionarErro("REG001", "Este e-mail já está em uso")
                                              .ComStatus(HttpStatusCode.BadRequest);
        }

        if (!string.IsNullOrEmpty(request.Documento) && await _repository.ExisteDocumentoAsync(request.Documento, cancellationToken))
        {
            return (CadastrarUsuarioResponse)response.AdicionarErro("REG002", "Este documento já está em uso")
                                              .ComStatus(HttpStatusCode.BadRequest);
        }

        if (!string.IsNullOrEmpty(request.Apelido) && await _repository.ExisteApelidoAsync(request.Apelido, cancellationToken))
        {
            return (CadastrarUsuarioResponse)response.AdicionarErro("REG003", "Este apelido já está em uso")
                                              .ComStatus(HttpStatusCode.BadRequest);
        }

        // 2. Criar model (Lógica de hash entraria aqui)
        var novoUsuario = new UsuarioModel
        {
            Nome = request.Nome,
            Email = request.Email,
            Documento = request.Documento,
            Apelido = request.Apelido,
            SenhaHash = request.Senha // Aqui entrará a lógica de hash proprietária
        };

        // 3. Salvar
        await _repository.AdicionarAsync(novoUsuario, cancellationToken);

        response.Id = novoUsuario.Id;
        return (CadastrarUsuarioResponse)response.ComStatus(HttpStatusCode.Created)
                                              .ComMensagem("Usuário cadastrado com sucesso");
    }
}

