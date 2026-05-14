using Core_Logs.Commands;

namespace app_backend_lista_compras.domain.Commands.ListaCompras.Response
{
	public class ItemListaAdicionarResponse : BaseCommand
	{
		public ItemListaResponse Item { get; set; } = new();
	}
}
