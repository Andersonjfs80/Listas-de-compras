using Core_Logs.Commands;

namespace app_backend_autenticacao.domain.Commands.Auth.Responses;

public class CadastrarUsuarioResponse : BaseCommand
{
    public Guid Id { get; set; }
}

