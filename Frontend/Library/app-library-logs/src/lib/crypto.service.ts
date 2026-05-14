import { Injectable, inject } from '@angular/core';
import * as jose from 'jose';
import { LOG_CONFIG } from './log.models';
import type { LogConfig } from './log.models';

@Injectable({
  providedIn: 'root'
})
export class CryptoService {
  private config = inject<LogConfig>(LOG_CONFIG);

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
      console.error('[CryptoService] Erro ao criptografar JOSE:', error);
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
      console.error('[CryptoService] Erro ao descriptografar JOSE:', error);
      return jwe;
    }
  }
}
