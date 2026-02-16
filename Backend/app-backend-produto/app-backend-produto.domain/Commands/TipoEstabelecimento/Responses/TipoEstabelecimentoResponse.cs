namespace app_backend_produto.domain.Commands.TipoEstabelecimento.Responses;

public class TipoEstabelecimentoResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; }
}
