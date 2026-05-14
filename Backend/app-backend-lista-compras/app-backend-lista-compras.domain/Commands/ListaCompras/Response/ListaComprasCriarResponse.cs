using Core_Logs.Commands;

namespace app_backend_lista_compras.domain.Commands.ListaCompras.Response
{
	public class ListaComprasCriarResponse : BaseCommand
	{
		public ListaComprasResponse Lista { get; set; } = new();
	}
}
