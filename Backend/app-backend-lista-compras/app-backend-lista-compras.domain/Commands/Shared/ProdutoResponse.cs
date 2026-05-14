using System;

namespace app_backend_lista_compras.domain.Commands.Shared
{
    public class ProdutoResponse
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string NomeCurto { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public ProdutoImagemModel Imagem { get; set; } = new();
        public ProdutoCodigoModel Codigo { get; set; } = new();
        public ProdutoCategoriaModel Categoria { get; set; } = new();
        public TipoPrecoModel TabelaPreco { get; set; } = new();
        public ProdutoPrecoModel Preco { get; set; } = new();
    }

    public class ProdutoImagemModel
    {
        public Guid Id { get; set; }
        public string Conteudo { get; set; } = string.Empty;
        public bool Favorito { get; set; }
    }

    public class ProdutoCodigoModel
    {
        public Guid Id { get; set; }
        public string CodigoProduto { get; set; } = string.Empty;
        public string CodigoBarras { get; set; } = string.Empty;
    }

    public class ProdutoCategoriaModel
    {
        public Guid Id { get; set; }
        public Guid CategoriaId { get; set; }
    }

    public class TipoPrecoModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }

    public class ProdutoPrecoModel
    {
        public Guid Id { get; set; }
        public decimal Valor { get; set; }
        public bool Principal { get; set; }
    }

    public class UnidadeMedidaModel
    {
        public Guid Id { get; set; }
        public string Sigla { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal FatorConversao { get; set; } = 1;
    }
}
