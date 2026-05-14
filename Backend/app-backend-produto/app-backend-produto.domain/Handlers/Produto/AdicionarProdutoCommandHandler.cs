using MediatR;
using app_backend_produto.domain.Commands.Produto.Requests;
using app_backend_produto.domain.Commands.Produto.Responses;
using app_backend_produto.domain.Interfaces.Repositories;
using app_backend_produto.domain.Models;

namespace app_backend_produto.domain.Handlers.Produto;

/// <summary>
/// Handler para criação de novos produtos com associações N-N
/// </summary>
public class AdicionarProdutoCommandHandler : IRequestHandler<AdicionarProdutoCommandRequest, AdicionarProdutoCommandResponse>
{
    private readonly IProdutoRepository _repository;

    public AdicionarProdutoCommandHandler(IProdutoRepository repository)
    {
        _repository = repository;
    }

    public async Task<AdicionarProdutoCommandResponse> Handle(
        AdicionarProdutoCommandRequest request, 
        CancellationToken cancellationToken)
    {
        var response = new AdicionarProdutoCommandResponse();

        try 
        {
            var produto = new ProdutoModel
            {
                Id = Guid.NewGuid(),
                Nome = request.Nome,
                NomeCurto = request.NomeCurto,
                Ativo = request.Ativo,
                DataCadastro = DateTime.UtcNow,
                UsuarioId = request.Headers.UsuarioId ?? Guid.Empty
            };

            // Mapear categorias vinculadas
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

            var result = await _repository.AdicionarAsync(produto, cancellationToken);
            
            response.Id = result.Id;
            response.ComMensagem("Produto adicionado com sucesso");
        }
        catch (Exception ex)
        {
            response.ComMensagemErro($"Erro ao adicionar produto: {ex.Message}");
        }

        return response;     
    }
}
