using System;
using System.Collections.Generic;

namespace app_backend_produto.domain.Models;

public class FornecedorModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty; // Razão Social
    public string NomeFantasia { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    
    public Guid UsuarioId { get; set; }
    
    // FK
    public Guid TipoEstabelecimentoId { get; set; }
    public TipoEstabelecimentoModel TipoEstabelecimento { get; set; } = null!;
    
    // Navegação
    public ICollection<ProdutoCodigoModel> Codigos { get; set; } = new List<ProdutoCodigoModel>();
}
