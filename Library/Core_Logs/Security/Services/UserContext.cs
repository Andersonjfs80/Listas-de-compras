using Microsoft.AspNetCore.Http;
using Core_Logs.Security.Interfaces;
using Core_Logs.Security.Models;
using System.Security.Claims;

namespace Core_Logs.Security.Services;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
    public string? Documento => _httpContextAccessor.HttpContext?.User?.FindFirstValue("Documento");
    public string? Apelido => _httpContextAccessor.HttpContext?.User?.FindFirstValue("Apelido");
    
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public UserSession? GetSession()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || !user.Identity?.IsAuthenticated == true) return null;

        return new UserSession
        {
            Id = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty,
            NomeExibicao = user.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
            Email = user.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            Documento = user.FindFirstValue("Documento") ?? string.Empty,
            Apelido = user.FindFirstValue("Apelido") ?? string.Empty,
            SiglaAplicacao = user.FindFirstValue("AppSigla") ?? string.Empty,
            Roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
        };
    }
}
