namespace app_backend_seguranca.domain.Interfaces.Services;

public interface IWhatsAppProvider
{
    Task<bool> EnviarWhatsAppAsync(string celular, string mensagem, CancellationToken cancellationToken);
}

