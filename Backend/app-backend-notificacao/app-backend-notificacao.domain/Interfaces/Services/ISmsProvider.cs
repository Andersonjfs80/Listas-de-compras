namespace app_backend_notificacao.domain.Interfaces.Services;

public interface ISmsProvider
{
    Task<bool> EnviarSmsAsync(string celular, string mensagem, CancellationToken cancellationToken);
}

