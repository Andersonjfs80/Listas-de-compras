using Core_Logs.Commands;

namespace app_backend_produto.domain.Commands.UnidadeMedida.Responses;

/// <summary>
/// Response padronizada para listagem de unidades de medida.
/// </summary>
public class ListarUnidadesMedidaQueryResponse : BaseCommand
{
    public IEnumerable<UnidadeMedidaResponse> Unidades { get; set; } = [];
}
