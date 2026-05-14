using MediatR;
using app_backend_produto.domain.Commands.UnidadeMedida.Responses;
using Core_Logs.Models.Request;
using System.Text.Json.Serialization;

namespace app_backend_produto.domain.Commands.UnidadeMedida.Requests;

public class ListarUnidadesMedidaQueryRequest : IRequest<ListarUnidadesMedidaQueryResponse>
{
    [JsonIgnore]
    public RequestHeaders Headers { get; set; } = new();
}
