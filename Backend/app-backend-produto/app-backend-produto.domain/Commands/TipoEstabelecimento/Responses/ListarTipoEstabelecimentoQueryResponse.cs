using Core_Logs.Commands;

namespace app_backend_produto.domain.Commands.TipoEstabelecimento.Responses;

/// <summary>
/// Response padronizada para listagem de tipos de estabelecimento.
/// </summary>
public class ListarTipoEstabelecimentoQueryResponse : BaseCommand
{
    public IEnumerable<TipoEstabelecimentoResponse> Tipos { get; set; } = new List<TipoEstabelecimentoResponse>();
}
