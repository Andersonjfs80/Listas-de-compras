using System.Text.RegularExpressions;

namespace Core_Logs.Log;

/// <summary>
/// Utilitário para saneamento de strings JSON antes do log.
/// </summary>
public static class JsonSanitizer
{
    private const string Mask = "***";

    /// <summary>
    /// Ofusca os valores das chaves informadas em uma string JSON.
    /// </summary>
    public static string Sanitize(string json, List<string> keysToObfuscate)
    {
        if (string.IsNullOrWhiteSpace(json) || keysToObfuscate == null || keysToObfuscate.Count == 0)
            return json;

        try
        {
            var sanitized = json;
            foreach (var key in keysToObfuscate)
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
