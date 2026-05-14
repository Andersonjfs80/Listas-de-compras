using System;

namespace Core_Logs.Commands;

/// <summary>
/// Objeto padrão da biblioteca Core para retorno de dados de paginação nas respostas
/// </summary>
public class PaginacaoInfoResponse
{
    public int TotalItems { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalItems / (double)PageSize) : 0;
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}
