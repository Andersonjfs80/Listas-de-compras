using Core_Logs.Commands;

namespace app_backend_seguranca.domain.Commands.Messaging.Responses;

public class ValidarSmsResponse : BaseCommand
{
    public bool Sucesso { get; set; }
}

