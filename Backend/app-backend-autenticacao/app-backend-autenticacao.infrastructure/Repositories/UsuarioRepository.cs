using app_backend_autenticacao.domain.Interfaces.Repositories;
using app_backend_autenticacao.domain.Models;
using app_backend_autenticacao.infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

namespace app_backend_autenticacao.infrastructure.Repositories;

public class UsuarioRepository(AppDbContext context) : IUsuarioRepository
{
    private readonly AppDbContext _context = context;

    public async Task<UsuarioModel?> ObterPorEmailAsync(string email, CancellationToken ct)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public async Task<UsuarioModel?> ObterPorIdentificadorAsync(string identificador, CancellationToken ct)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => 
            u.Email == identificador || 
            u.Documento == identificador || 
            u.Apelido == identificador, ct);
    }

    public async Task AdicionarAsync(UsuarioModel usuario, CancellationToken ct)
    {
        await _context.Usuarios.AddAsync(usuario, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> ExisteEmailAsync(string email, CancellationToken ct)
    {
        return await _context.Usuarios.AnyAsync(u => u.Email == email, ct);
    }

    public async Task<bool> ExisteDocumentoAsync(string documento, CancellationToken ct)
    {
        return await _context.Usuarios.AnyAsync(u => u.Documento == documento, ct);
    }

    public async Task<bool> ExisteApelidoAsync(string apelido, CancellationToken ct)
    {
        return await _context.Usuarios.AnyAsync(u => u.Apelido == apelido, ct);
    }

    public async Task AtualizarAsync(UsuarioModel usuario, CancellationToken ct)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync(ct);
    }
}

