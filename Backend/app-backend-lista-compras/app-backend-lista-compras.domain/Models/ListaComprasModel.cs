namespace app_backend_lista_compras.domain.Models;

public class ListaComprasModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public Guid UsuarioId { get; set; }
    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; } = true;

    // Navegação
    public ICollection<ItemListaModel> Itens { get; set; } = new List<ItemListaModel>();
}
