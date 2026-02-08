using app_backend_seguranca.domain.Interfaces.Services;

namespace app_backend_seguranca.infrastructure.Services;

public class MessagingService(
    IWhatsAppProvider whatsAppProvider) : IMessagingService
{
    private readonly IWhatsAppProvider _whatsAppProvider = whatsAppProvider;

    public async Task<bool> EnviarCodigoAsync(string destino, string codigo, CancellationToken cancellationToken)
    {
        var mensagem = $"Seu código de verificação é: {codigo}";

        // Lógica simples: se começa com (ou contém) características de celular, tenta WhatsApp
        // Em um cenário real, você teria um enum ou flag no request para escolher o canal.
        return await _whatsAppProvider.EnviarWhatsAppAsync(destino, mensagem, cancellationToken);
    }

    public async Task<bool> ValidarCodigoAsync(string destino, string codigo, CancellationToken cancellationToken)
    {
        // Aceita 123456 como código de teste universal para o ambiente de dev
        return await Task.FromResult(codigo == "123456");
    }
}

