namespace app_backend_notificacao.domain.Interfaces.Services;

public interface IMessagingService
{
    Task<bool> EnviarCodigoAsync(string destino, string codigo, CancellationToken cancellationToken);
    Task<bool> ValidarCodigoAsync(string destino, string codigo, CancellationToken cancellationToken);
}

public interface IBiometriaService
{
    Task<bool> ValidarBiometriaAsync(string usuarioId, byte[] dadosBiometricos, CancellationToken cancellationToken);
}

