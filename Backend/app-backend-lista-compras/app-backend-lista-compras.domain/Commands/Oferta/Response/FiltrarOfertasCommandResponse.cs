using Core_Logs.Commands;
using app_backend_lista_compras.domain.Commands.Shared;

namespace app_backend_lista_compras.domain.Commands.Oferta.Response
{
	public class FiltrarOfertasCommandResponse : BaseCommand
    {
        public List<OfertaResponse> Ofertas { get; set; } = new();
        public PaginacaoInfoResponse Paginacao { get; set; } = new();
    }

    public class OfertaResponse
    {
        public Guid Id { get; set; }
        public DateTime DataFim { get; set; }
        public ProdutoResponse Produto { get; set; } = new();
    }
}
