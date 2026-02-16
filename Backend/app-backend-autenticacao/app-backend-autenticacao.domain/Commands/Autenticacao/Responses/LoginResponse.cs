using Core_Logs.Commands;

namespace app_backend_autenticacao.domain.Commands.Autenticacao.Responses;

public class LoginResponse : BaseCommand
{
    public string Token { get; set; } = string.Empty;
    public string Type { get; set; } = "Bearer";
    public DateTime? ExpiresIn { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public UsuarioDto Usuario { get; set; } = new();
}

public class UsuarioDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

