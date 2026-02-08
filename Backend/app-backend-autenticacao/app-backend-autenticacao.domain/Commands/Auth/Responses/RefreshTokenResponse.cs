using Core_Logs.Commands;

namespace app_backend_autenticacao.domain.Commands.Auth.Responses;

public class RefreshTokenResponse : BaseCommand
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

