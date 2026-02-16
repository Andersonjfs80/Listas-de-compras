using System;

using Core_Logs.Security.Models;

namespace Core_Logs.Interfaces;

/// <summary>
/// Serviço central de segurança para criptografia de senhas e dados.
/// </summary>
public interface ISecurityService
{
    // --- Gestão de Senhas (Identidade) ---
    
    /// <summary>
    /// Gera um hash seguro para a senha informada.
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verifica se a senha informada corresponde ao hash.
    /// </summary>
    bool VerifyPassword(string password, string hashedPassword);

    /// <summary>
    /// Obtém o status da senha com base na data da última troca.
    /// </summary>
    PasswordStatus GetPasswordStatus(DateTime lastChangeDate);

    /// <summary>
    /// Obtém a quantidade de dias restantes até a expiração da senha.
    /// </summary>
    int GetDaysToPasswordExpire(DateTime lastChangeDate);

    // --- Criptografia de Dados (Tipo 1: Fixo/Permanente) ---

    /// <summary>
    /// Criptografa dados de forma permanente (ex: para banco de dados) usando uma chave mestra.
    /// </summary>
    string EncryptFixed(string plainText);

    /// <summary>
    /// Descriptografa dados permanentes.
    /// </summary>
    string DecryptFixed(string cipherText);

    // --- Criptografia de Dados (Tipo 2: Sensível/Transição/Sessão) ---

    /// <summary>
    /// Criptografa dados sensíveis para trânsito em rede, vinculados a uma sessão.
    /// </summary>
    string EncryptSession(string plainText, string sessionId);

    /// <summary>
    /// Descriptografa dados sensíveis vinculados a uma sessão.
    /// </summary>
    string DecryptSession(string cipherText, string sessionId);

    // --- Gestão de Chaves e Handshake ---

    /// <summary>
    /// Gera um par de chave e contra-chave temporária para a sessão.
    /// </summary>
    (string Key, string CounterKey) GenerateSessionKeys(string sessionId);
}
