using System.Text.RegularExpressions;

namespace app_backend_autenticacao.domain.Helpers;

public enum ForcaSenha
{
    Fraca = 1,
    Media = 2,
    Forte = 3
}

public static class PasswordHelper
{
    private const int MinimoCaracteres = 8;

    public static (bool Valido, string Mensagem, ForcaSenha Forca) ValidarSenha(string senha, ForcaSenha forcaMinimaRequerida = ForcaSenha.Fraca)
    {
        if (string.IsNullOrWhiteSpace(senha))
            return (false, "A senha não pode ser vazia.", ForcaSenha.Fraca);

        if (senha.Length < MinimoCaracteres)
            return (false, $"A senha deve ter no mínimo {MinimoCaracteres} caracteres.", ForcaSenha.Fraca);

        if (PossuiCaracteresRepetidosExcessivos(senha))
            return (false, "A senha possui caracteres repetidos em sequência, o que a torna insegura.", ForcaSenha.Fraca);

        var forca = CalcularForca(senha);

        if (forca < forcaMinimaRequerida)
        {
            var mensagem = forcaMinimaRequerida switch
            {
                ForcaSenha.Media => "A senha deve ser no mínimo 'Média' (letras e números).",
                ForcaSenha.Forte => "A senha deve ser 'Forte' (letras, números, letras maiúsculas e símbolos).",
                _ => "Senha inválida."
            };
            return (false, mensagem, forca);
        }

        return (true, string.Empty, forca);
    }

    private static ForcaSenha CalcularForca(string senha)
    {
        int score = 0;

        // Critérios simples
        if (Regex.IsMatch(senha, @"[a-z]")) score++; // Tem minúsculas
        if (Regex.IsMatch(senha, @"[0-9]")) score++; // Tem números
        if (Regex.IsMatch(senha, @"[A-Z]")) score++; // Tem maiúsculas
        if (Regex.IsMatch(senha, @"[!@#$%^&*(),.?/|""':{}|<>]")) score++; // Tem símbolos

        if (score <= 2) return ForcaSenha.Fraca;
        if (score == 3) return ForcaSenha.Media;
        return ForcaSenha.Forte;
    }

    private static bool PossuiCaracteresRepetidosExcessivos(string senha)
    {
        // Verifica se existem 3 ou mais caracteres idênticos em sequência (ex: "aaa")
        return Regex.IsMatch(senha, @"(.)\1\2");
    }
}

