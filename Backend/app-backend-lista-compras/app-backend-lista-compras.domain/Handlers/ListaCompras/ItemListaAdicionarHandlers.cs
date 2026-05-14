using MediatR;
using app_backend_lista_compras.domain.Interfaces.Repositories;
using app_backend_lista_compras.domain.Models;
using app_backend_lista_compras.domain.Commands.ListaCompras.Request;
using app_backend_lista_compras.domain.Commands.ListaCompras.Response;
using Mapster;

namespace app_backend_lista_compras.domain.Handlers.ListaCompras;

public class ItemListaAdicionarHandler : IRequestHandler<ItemListaAdicionarRequest, ItemListaAdicionarResponse>
{
    private readonly IItemListaRepository _repository;
    private readonly IListaComprasRepository _listaRepository;

    public ItemListaAdicionarHandler(
        IItemListaRepository repository,
        IListaComprasRepository listaRepository)
    {
        _repository = repository;
        _listaRepository = listaRepository;
    }

    public async Task<ItemListaAdicionarResponse> Handle(
        ItemListaAdicionarRequest request,
        CancellationToken cancellationToken)
    {
        var lista = await _listaRepository.ObterPorIdAsync(request.Item.ListaId, cancellationToken);
        if (lista == null || lista.UsuarioId != request.Headers.UsuarioId.GetValueOrDefault())
            throw new UnauthorizedAccessException("Lista não encontrada ou sem permissão.");

        var item = new ItemListaModel
        {
            Id = Guid.NewGuid(),
            ListaId = request.Item.ListaId,
            ProdutoId = request.Item.Produto.Id != Guid.Empty ? request.Item.Produto.Id : null,
            NomeProduto = request.Item.NomeProduto,
            NomeCurto = request.Item.NomeCurto,
            Quantidade = request.Item.Quantidade,
            UnidadeMedida = request.Item.UnidadeMedida.Sigla,
            QuantidadeConversao = request.Item.QuantidadeConversao,
            UnidadeMedidaConversao = request.Item.UnidadeMedidaConversao.Sigla,
            PrecoCompra = request.Item.PrecoCompra,
            PrecoVenda = request.Item.PrecoVenda,
            CategoriaId = request.Item.Produto.Categoria?.CategoriaId != Guid.Empty ? request.Item.Produto.Categoria?.CategoriaId : null,
            Imagem = request.Item.Produto.Imagem?.Conteudo,
            Marcado = false,
            DataCadastro = DateTime.UtcNow
        };

        var adicionado = await _repository.AdicionarAsync(item, cancellationToken);

        var response = new ItemListaAdicionarResponse { Item = adicionado.Adapt<ItemListaResponse>() };
        response.ComMensagem("Item adicionado à lista com sucesso");
        return response;
    }
}

