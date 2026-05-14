using MediatR;
using app_backend_produto.domain.Commands.Produto.Requests;
using app_backend_produto.domain.Commands.Produto.Responses;
using app_backend_produto.domain.Interfaces.Repositories;
using app_backend_produto.domain.Models;

namespace app_backend_produto.domain.Handlers.Produto;

/// <summary>
/// Handler para atualização de produtos e sincronização de categorias N-N
/// </summary>
public class AtualizarProdutoCommandHandler : IRequestHandler<AtualizarProdutoCommandRequest, AtualizarProdutoCommandResponse>
{
    private readonly IProdutoRepository _repository;

    public AtualizarProdutoCommandHandler(IProdutoRepository repository)
    {
        _repository = repository;
    }

    public async Task<AtualizarProdutoCommandResponse> Handle(
        AtualizarProdutoCommandRequest request, 
        CancellationToken cancellationToken)
    {
        var response = new AtualizarProdutoCommandResponse();

        try 
        {
            var produto = await _repository.ObterPorIdAsync(request.Id, cancellationToken);
            if (produto == null)
            {
                response.ComMensagemErro("Produto não encontrado.");
                return response;
            }

            // Atualizar campos básicos
            produto.Nome = request.Nome;
            produto.NomeCurto = request.NomeCurto;
            produto.Ativo = request.Ativo;

            // Lógica de inativação
            if (!request.Ativo && produto.DataInativacao == null)
                produto.DataInativacao = DateTime.UtcNow;
            else if (request.Ativo)
                produto.DataInativacao = null;

            // Sincronização de categorias (N-N)
            // Abordagem: Limpar associações atuais e adicionar as novas
            produto.ProdutoCategorias.Clear();

            if (request.Categorias != null)
            {
                foreach (var item in request.Categorias)
                {
                    produto.ProdutoCategorias.Add(new ProdutoCategoriaModel
                    {
                        ProdutoId = produto.Id,
                        CategoriaId = item.CategoriaId,
                        Tipo = item.Tipo,
                        Ativo = true,
                        DataCadastro = DateTime.UtcNow,
                        UsuarioId = request.Headers.UsuarioId ?? Guid.Empty
                    });
                }
            }

            await _repository.(produto, cancellationToken);
            
            response.ComMensagem("Produto atualizado com sucesso.");
        }
        catch (Exception ex)
        {
            response.ComMensagemErro($"Erro ao atualizar produto: {ex.Message}");
        }

        return response;
    }
}
