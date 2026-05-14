using MediatR;
using app_backend_lista_compras.domain.Interfaces.Repositories;
using app_backend_lista_compras.domain.Commands.Oferta.Request;
using app_backend_lista_compras.domain.Commands.Oferta.Response;
using Mapster;
using Core_Logs.Commands;

namespace app_backend_lista_compras.domain.Handlers.Oferta;

public class FiltrarOfertasCommandHandler : IRequestHandler<FiltrarOfertasCommandRequest, FiltrarOfertasCommandResponse>
{
    private readonly IOfertaRepository _repository;

    public FiltrarOfertasCommandHandler(IOfertaRepository repository)
    {
        _repository = repository;
    }

    public async Task<FiltrarOfertasCommandResponse> Handle(FiltrarOfertasCommandRequest request, CancellationToken cancellationToken)
    {
        if (request.PageNumber < 1) request.PageNumber = 1;
        if (request.PageSize < 1) request.PageSize = 20;
        if (request.PageSize > 100) request.PageSize = 100;

        var (items, totalCount) = await _repository.ObterAtivasAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var response = new FiltrarOfertasCommandResponse
        {
            Ofertas = items.Adapt<List<OfertaResponse>>(),
            Paginacao = new PaginacaoInfoResponse
            {
                TotalItems = totalCount,
                CurrentPage = request.PageNumber,
                PageSize = request.PageSize
            }
        };

        response.ComMensagem("Ofertas recuperadas com sucesso");
        return response;
    }
}
