using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Core_Logs.Interfaces;
using Core_Logs.Security.Models;
using Microsoft.Extensions.Options;
using BCrypt.Net;

namespace Core_Logs.Implementation;

public class SecurityService : ISecurityService
{
    private readonly SecuritySettings _settings;

    public SecurityService(IOptions<SecuritySettings> settings)
    {
        _settings = settings.Value;
    }

    // --- Gestão de Senhas (Identidade) ---

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch
        {
            return false;
        }
    }

    public PasswordStatus GetPasswordStatus(DateTime lastChangeDate)
    {
        var daysRemaining = GetDaysToPasswordExpire(lastChangeDate);

        if (daysRemaining <= 0) return PasswordStatus.Expired;
        if (daysRemaining <= _settings.PasswordWarningDays) return PasswordStatus.Warning;

        return PasswordStatus.Valid;
    }

    public int GetDaysToPasswordExpire(DateTime lastChangeDate)
    {
        var expirationDate = lastChangeDate.AddDays(_settings.PasswordExpirationDays);
        var remaining = (expirationDate - DateTime.UtcNow).TotalDays;
        return (int)Math.Max(0, Math.Ceiling(remaining));
    }

    // --- Criptografia de Dados (Tipo 1: Fixo/Permanente) ---

    public string EncryptFixed(string plainText)
    {
        if (string.IsNullOrEmpty(plainText)) return plainText;
        return Encrypt(plainText, _settings.FixedEncryptionKey);
    }

    public string DecryptFixed(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText)) return cipherText;
        return Decrypt(cipherText, _settings.FixedEncryptionKey);
    }

    // --- Criptografia de Dados (Tipo 2: Sensível/Transição/Sessão) ---

    public string EncryptSession(string plainText, string sessionId)
    {
        if (!_settings.EnableBodyEncryption) return plainText;
        if (string.IsNullOrEmpty(plainText)) return plainText;
        
        string sessionKey = DeriveKeyFromSession(sessionId);
        return Encrypt(plainText, sessionKey);
    }

    public string DecryptSession(string cipherText, string sessionId)
    {
        if (!_settings.EnableBodyEncryption) return cipherText; 
        if (string.IsNullOrEmpty(cipherText)) return cipherText;

        string sessionKey = DeriveKeyFromSession(sessionId);
        return Decrypt(cipherText, sessionKey);
    }

    // --- Gestão de Chaves e Handshake ---

    public (string Key, string CounterKey) GenerateSessionKeys(string sessionId)
    {
        string key = DeriveKeyFromSession(sessionId);
        string counterKey = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(key + "_counter")));
        
        return (key, counterKey);
    }

    // --- Métodos Privados Auxiliares (AES-256) ---

    private string DeriveKeyFromSession(string sessionId)
    {
        using var sha256 = SHA256.Create();
        byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(sessionId + _settings.SecretKey));
        return Convert.ToBase64String(hash);
    }

    private string Encrypt(string plainText, string base64Key)
    {
        try
        {
            byte[] key = Ensure32Bytes(base64Key);
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.GenerateIV();
            byte[] iv = aesAlg.IV;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msEncrypt = new MemoryStream();
            msEncrypt.Write(iv, 0, iv.Length); 

            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }

            return Convert.ToBase64String(msEncrypt.ToArray());
        }
        catch
        {
            return plainText; 
        }
    }

    private string Decrypt(string cipherText, string base64Key)
    {
        try
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);
            byte[] key = Ensure32Bytes(base64Key);
            
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = key;

            byte[] iv = new byte[aesAlg.BlockSize / 8];
            byte[] cipher = new byte[fullCipher.Length - iv.Length];

            Array.Copy(fullCipher, iv, iv.Length);
            Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            aesAlg.IV = iv;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new MemoryStream(cipher);
            using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();
        }
        catch
        {
            return cipherText;
        }
    }

    private byte[] Ensure32Bytes(string base64Key)
    {
        if (string.IsNullOrEmpty(base64Key)) 
            return SHA256.HashData(Encoding.UTF8.GetBytes("DefaultKeyPlaceholder"));

        try
        {
            byte[] key = Convert.FromBase64String(base64Key);
            if (key.Length == 32) return key;
            return SHA256.HashData(key); 
        }
        catch
        {
            return SHA256.HashData(Encoding.UTF8.GetBytes(base64Key));
        }
    }
}
