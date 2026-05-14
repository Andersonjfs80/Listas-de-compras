using Core_Logs.Commands;

namespace app_backend_produto.domain.Commands.TipoEstabelecimento.Responses;

public class TipoEstabelecimentoResponse : BaseCommand
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; }
}
