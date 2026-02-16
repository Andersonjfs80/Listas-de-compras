namespace app_backend_notificacao.domain.Interfaces.Services;

public interface IWhatsAppProvider
{
    Task<bool> EnviarWhatsAppAsync(string celular, string mensagem, CancellationToken cancellationToken);
}

