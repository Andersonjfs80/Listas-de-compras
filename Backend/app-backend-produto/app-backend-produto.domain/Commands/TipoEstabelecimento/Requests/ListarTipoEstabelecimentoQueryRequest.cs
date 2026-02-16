using MediatR;
using app_backend_produto.domain.Commands.TipoEstabelecimento.Responses;
using Core_Logs.Models.Request;
using System.Text.Json.Serialization;

namespace app_backend_produto.domain.Commands.TipoEstabelecimento.Requests;

public class ListarTipoEstabelecimentoQueryRequest : IRequest<ListarTipoEstabelecimentoQueryResponse>
{
    [JsonIgnore]
    public RequestHeaders Headers { get; set; } = new();
}
