using app_backend_autenticacao.domain.Models;

namespace app_backend_autenticacao.domain.Interfaces.Repositories;

public interface IUsuarioRepository
{
    Task<UsuarioModel?> ObterPorEmailAsync(string email, CancellationToken ct);
    Task<UsuarioModel?> ObterPorIdentificadorAsync(string identificador, CancellationToken ct);
    Task AdicionarAsync(UsuarioModel usuario, CancellationToken ct);
    Task<bool> ExisteEmailAsync(string email, CancellationToken ct);
    Task<bool> ExisteDocumentoAsync(string documento, CancellationToken ct);
    Task<bool> ExisteApelidoAsync(string apelido, CancellationToken ct);
    Task AtualizarAsync(UsuarioModel usuario, CancellationToken ct);
}

