using System.Text.RegularExpressions;

namespace Core_Logs.Log;

/// <summary>
/// Utilitário para saneamento de strings JSON antes do log.
/// </summary>
public static class JsonSanitizer
{
    private const string Mask = "***";
    private static readonly List<string> DefaultSensitiveKeys = new() { "senha", "password", "token", "secret", "key", "senhaAcesso" };

    /// <summary>
    /// Ofusca os valores das chaves informadas em uma string JSON.
    /// </summary>
    public static string Sanitize(string json, List<string>? keysToObfuscate)
    {
        if (string.IsNullOrWhiteSpace(json))
            return json;

        var allKeysToObfuscate = DefaultSensitiveKeys.ToList();
        if (keysToObfuscate != null)
        {
            allKeysToObfuscate.AddRange(keysToObfuscate.Where(k => !allKeysToObfuscate.Contains(k)));
        }

        try
        {
            var sanitized = json;
            foreach (var key in allKeysToObfuscate)
            {
                // Regex para encontrar "chave": "valor" ou "chave":"valor"
                // Grupos: 1: "chave":", 2: valor, 3: "
                var pattern = $@"(""{key}""\s*:\s*"")([^""]*)("")";
                sanitized = Regex.Replace(sanitized, pattern, $"$1{Mask}$3", RegexOptions.IgnoreCase);
            }
            return sanitized;
        }
        catch
        {
            // Em caso de erro no Regex (ex: JSON muito grande ou mal formatado), 
            // retorna o original para não quebrar o pipeline de log
            return json;
        }
    }
}
