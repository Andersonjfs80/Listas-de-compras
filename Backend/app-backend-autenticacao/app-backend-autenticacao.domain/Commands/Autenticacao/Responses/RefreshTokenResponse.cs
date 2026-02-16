using Core_Logs.Commands;

namespace app_backend_autenticacao.domain.Commands.Autenticacao.Responses;

public class RefreshTokenResponse : BaseCommand
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

