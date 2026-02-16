using System;
using System.ComponentModel.DataAnnotations;
using app_backend_produto.domain.Enums;

namespace app_backend_produto.domain.Models;

public class ProdutoCategoriaModel
{
    public Guid Id { get; set; } = Guid.NewGuid(); // PK para N-N caso necessário, ou Composite Key

    public Guid ProdutoId { get; set; }
    public virtual ProdutoModel Produto { get; set; } = null!;

    public Guid CategoriaId { get; set; }
    public virtual CategoriaModel Categoria { get; set; } = null!;

    public TipoCategoria Tipo { get; set; } // Principal ou Adicional
    
    public bool Ativo { get; set; } = true;
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    public DateTime? DataInativacao { get; set; }
    public Guid UsuarioId { get; set; }
}
