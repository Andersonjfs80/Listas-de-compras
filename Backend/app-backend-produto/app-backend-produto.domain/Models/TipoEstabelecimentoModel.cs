using System;
using System.Collections.Generic;

namespace app_backend_produto.domain.Models;

public class TipoEstabelecimentoModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    
    public Guid UsuarioId { get; set; }
    
    // Navegação
    public ICollection<FornecedorModel> Fornecedores { get; set; } = new List<FornecedorModel>();
}
