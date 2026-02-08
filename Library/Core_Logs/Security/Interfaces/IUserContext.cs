using Core_Logs.Security.Models;

namespace Core_Logs.Security.Interfaces;

public interface IUserContext
{
    string? UserId { get; }
    string? Email { get; }
    string? Documento { get; }
    string? Apelido { get; }
    bool IsAuthenticated { get; }
    UserSession? GetSession();
}
