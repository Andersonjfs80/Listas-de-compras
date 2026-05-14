using Mapster;
using app_backend_produto.domain.Enums;
using app_backend_produto.domain.Models;
using app_backend_produto.domain.Commands.Produto.Responses;

namespace app_backend_produto.infrastructure.Mappings;

public class ProdutoMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ProdutoModel, ProdutoResponse>()
            // Mapeia Codigo (primeiro ativo que encontrar na lista)
            .Map(dest => dest.Codigo,
                 src => src.ProdutoCodigos.FirstOrDefault(c => c.Ativo))
            
            // Mapeia Categoria (TipoPrincipal)
            .Map(dest => dest.Categoria,
                 src => src.ProdutoCategorias.FirstOrDefault(c => c.Ativo && c.Tipo == TipoCategoria.Principal))
            
            // Mapeia Preco Principal
            .Map(dest => dest.Preco,
                 src => src.ProdutoPrecos.FirstOrDefault(p => p.Ativo && p.Principal))
            
            // Mapeia TabelaPreco pegando o TipoPreco relacionado ao Preço Principal
            .Map(dest => dest.TabelaPreco,
                 src => src.ProdutoPrecos.FirstOrDefault(p => p.Ativo && p.Principal) != null 
                        ? src.ProdutoPrecos.FirstOrDefault(p => p.Ativo && p.Principal)!.TipoPreco 
                        : null)
            
            // Mapeia a Imagem Principal (Favorito e Ativo) ou a primeira Ativa
            .Map(dest => dest.Imagem,
                 src => src.ProdutoImagens.FirstOrDefault(i => i.Ativo && i.Favorito) ?? 
                        src.ProdutoImagens.FirstOrDefault(i => i.Ativo));
    }
}
