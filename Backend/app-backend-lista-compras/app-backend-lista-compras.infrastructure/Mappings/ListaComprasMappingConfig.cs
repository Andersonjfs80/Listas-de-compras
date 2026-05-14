using app_backend_lista_compras.domain.Commands.ListaCompras.Response;
using app_backend_lista_compras.domain.Commands.Oferta.Response;
using app_backend_lista_compras.domain.Commands.Shared;
using app_backend_lista_compras.domain.Models;
using Mapster;

namespace app_backend_lista_compras.infrastructure.Mappings;

public class ListaComprasMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // 1. Mapeamento de ListaComprasModel -> ListaComprasResponse
        config.NewConfig<ListaComprasModel, ListaComprasResponse>()
            .Map(dest => dest.TotalItens, src => src.Itens.Count)
            .Map(dest => dest.ItensMarcados, src => src.Itens.Count(i => i.Marcado))
            .Map(dest => dest.Itens, src => src.Itens);

        // 2. Mapeamento de ItemListaModel -> ItemListaResponse
        config.NewConfig<ItemListaModel, ItemListaResponse>()
            .Map(dest => dest.DataCadastro, src => src.DataCadastro)
            .Map(dest => dest.ListaId, src => src.ListaId)
            .Map(dest => dest.UsuarioId, src => src.Lista != null ? src.Lista.UsuarioId : Guid.Empty)
            .Map(dest => dest.UnidadeMedida.Sigla, src => src.UnidadeMedida)
            .Map(dest => dest.UnidadeMedidaConversao.Sigla, src => src.UnidadeMedidaConversao) 
            .Map(dest => dest.Produto.Id, src => src.ProdutoId)
            .Map(dest => dest.Produto.Nome, src => src.NomeProduto)
            .Map(dest => dest.Produto.NomeCurto, src => src.NomeCurto)
            .Map(dest => dest.Produto.Categoria.CategoriaId, src => src.CategoriaId)
            .Map(dest => dest.Produto.Imagem.Conteudo, src => src.Imagem);

        // 3. Mapeamento de OfertaModel -> OfertaResponse
        config.NewConfig<OfertaModel, OfertaResponse>()
            .Map(dest => dest.Produto.Id, src => src.ProdutoId)
            .Map(dest => dest.Produto.Nome, src => src.NomeProduto)
            .Map(dest => dest.Produto.Preco.Valor, src => src.PrecoAtual)
            .Map(dest => dest.Produto.Preco.Principal, src => true)
            .Map(dest => dest.Produto.Categoria.CategoriaId, src => src.CategoriaId)
            .Map(dest => dest.Produto.Imagem.Conteudo, src => src.Imagem);
    }
}
