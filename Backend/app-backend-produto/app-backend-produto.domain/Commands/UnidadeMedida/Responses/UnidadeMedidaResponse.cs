namespace app_backend_produto.domain.Commands.UnidadeMedida.Responses;

public class UnidadeMedidaResponse
{
    public Guid Id { get; set; }
    public string Sigla { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal FatorConversao { get; set; }
    public bool Ativo { get; set; }
}
