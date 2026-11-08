import { Injectable, inject } from '@angular/core';
import * as jose from 'jose';
import * as CryptoJS from 'crypto-js';
import { LOG_CONFIG } from './log.models';
import type { LogConfig } from './log.models';
import { LogService } from './log.service';

@Injectable({
  providedIn: 'root'
})
export class CryptoService {
  private config = inject<LogConfig>(LOG_CONFIG);
  private logService = inject(LogService);

  /**
   * Criptografa o corpo da requisição usando o padrão JOSE (JWE - JSON Web Encryption).
   * Utiliza algoritmo DIR (Direct Encryption) e A256GCM (AES GCM 256 bits).
   */
  async encryptBodyJose(payload: any): Promise<string> {
    if (!payload || !this.config.secretKey) {
      return typeof payload === 'string' ? payload : JSON.stringify(payload);
    }

    try {
      const bodyStr = typeof payload === 'string' ? payload : JSON.stringify(payload);
      const secret = new TextEncoder().encode(this.config.secretKey.padEnd(32, ' ').substring(0, 32));

      const jwe = await new jose.CompactEncrypt(new TextEncoder().encode(bodyStr))
        .setProtectedHeader({ alg: 'dir', enc: 'A256GCM' })
        .encrypt(secret);

      return jwe;
    } catch (error) {
      this.logService.error('[CryptoService] Erro ao criptografar JOSE:', 'CryptoService', error);
      return typeof payload === 'string' ? payload : JSON.stringify(payload);
    }
  }

  /**
   * Descriptografa um pacote JOSE (JWE) vindo do backend.
   */
  async decryptBodyJose(jwe: any): Promise<any> {
    // Se o backend enviar como JSON string "eyJ...", o Angular HttpClient pode já ter parseado ou não.
    const token = typeof jwe === 'string' ? jwe : JSON.stringify(jwe);
    
    if (!token || !token.includes('.') || !this.config.secretKey) return jwe;

    try {
      const secret = new TextEncoder().encode(this.config.secretKey.padEnd(32, ' ').substring(0, 32));
      const { plaintext } = await jose.compactDecrypt(token.replace(/"/g, ''), secret);
      const decoded = new TextDecoder().decode(plaintext);
      
      try {
        return JSON.parse(decoded);
      } catch {
        return decoded;
      }
    } catch (error) {
      this.logService.error('[CryptoService] Erro ao descriptografar JOSE:', 'CryptoService', error);
      return jwe;
    }
  }

  // --- Criptografia de Campos (Compatibilidade com .NET EncryptFixed) ---

  private ensure32Bytes(base64Key: string | undefined): CryptoJS.lib.WordArray {
    if (!base64Key) {
      return CryptoJS.SHA256("DefaultKeyPlaceholder");
    }

    const isBase64 = /^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?$/.test(base64Key);

    if (isBase64) {
      const key = CryptoJS.enc.Base64.parse(base64Key);
      if (key.sigBytes === 32) return key;
      return CryptoJS.SHA256(key);
    } else {
      return CryptoJS.SHA256(CryptoJS.enc.Utf8.parse(base64Key));
    }
  }

  public encryptFixed(plainText: string): string {
    if (!plainText) return plainText;
    if (this.config.enableFieldEncryption === false) return plainText;

    try {
      const key = this.ensure32Bytes(this.config.fixedEncryptionKey);
      const iv = CryptoJS.lib.WordArray.random(16);

      const encrypted = CryptoJS.AES.encrypt(plainText, key, {
        iv: iv,
        mode: CryptoJS.mode.CBC,
        padding: CryptoJS.pad.Pkcs7
      });

      const combined = iv.clone().concat(encrypted.ciphertext);
      return CryptoJS.enc.Base64.stringify(combined);
    } catch (e) {
      this.logService.error('[CryptoService] Erro ao criptografar campo (Fixed):', 'CryptoService', e);
      return plainText;
    }
  }

  public decryptFixed(cipherText: string): string {
    if (!cipherText) return cipherText;
    if (this.config.enableFieldEncryption === false) return cipherText;

    try {
      const key = this.ensure32Bytes(this.config.fixedEncryptionKey);
      const combined = CryptoJS.enc.Base64.parse(cipherText);

      if (combined.sigBytes <= 16) return cipherText;

      const ivWords = combined.words.slice(0, 4);
      const iv = CryptoJS.lib.WordArray.create(ivWords, 16);

      const cipherWords = combined.words.slice(4);
      const ciphertext = CryptoJS.lib.WordArray.create(cipherWords, combined.sigBytes - 16);

      const cipherParams = CryptoJS.lib.CipherParams.create({
        ciphertext: ciphertext
      });

      const decrypted = CryptoJS.AES.decrypt(cipherParams, key, {
        iv: iv,
        mode: CryptoJS.mode.CBC,
        padding: CryptoJS.pad.Pkcs7
      });

      const result = decrypted.toString(CryptoJS.enc.Utf8);
      return result || cipherText;
    } catch (e) {
      this.logService.error('[CryptoService] Erro ao descriptografar campo (Fixed):', 'CryptoService', e);
      return cipherText;
    }
  }
}
