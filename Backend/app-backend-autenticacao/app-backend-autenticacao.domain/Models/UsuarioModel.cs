namespace app_backend_autenticacao.domain.Models;

public class UsuarioModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Apelido { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public bool Ativo { get; set; } = true;

    // Controle de Sessão
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public DateTime? DataAtualizacaoSenha { get; set; }
    
    /// <summary>
    /// Armazena o histórico das últimas N senhas em formato JSON: ["hash1", "hash2"]
    /// </summary>
    public string? HistoricoSenhasJson { get; set; }
}

