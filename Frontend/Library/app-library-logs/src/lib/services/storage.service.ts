import { Injectable, inject, Injector } from '@angular/core';
import * as CryptoJS from 'crypto-js';
import { LOG_CONFIG } from '../log.models';
import type { LogConfig } from '../log.models';
import { LogService } from '../log.service';

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  private config = inject<LogConfig>(LOG_CONFIG);
  private injector = inject(Injector);

  private get logService(): LogService {
    return this.injector.get(LogService);
  }

  private get enableEncryption(): boolean {
    return this.config.enableStorageEncryption !== false;
  }

  private ensure32Bytes(base64Key: string | undefined): CryptoJS.lib.WordArray {
    if (!base64Key) {
      return CryptoJS.SHA256("DefaultStorageKeyPlaceholder");
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

  private encrypt(value: string): string {
    if (!this.enableEncryption || !value) return value;
    try {
      const key = this.ensure32Bytes(this.config.storageEncryptionKey);
      const iv = CryptoJS.lib.WordArray.random(16);
      const encrypted = CryptoJS.AES.encrypt(value, key, {
        iv: iv,
        mode: CryptoJS.mode.CBC,
        padding: CryptoJS.pad.Pkcs7
      });
      const combined = iv.clone().concat(encrypted.ciphertext);
      return CryptoJS.enc.Base64.stringify(combined);
    } catch (e) {
      this.logService.error('[StorageService] Erro ao criptografar dados para storage:', 'StorageService', e);
      return value;
    }
  }

  private decrypt(cipherText: string): string {
    if (!this.enableEncryption || !cipherText) return cipherText;
    try {
      // Se não parecer um base64 válido ou não iniciar com IV, pode ser legado (não criptografado)
      if (!/^[A-Za-z0-9+/=]+$/.test(cipherText)) return cipherText;

      const key = this.ensure32Bytes(this.config.storageEncryptionKey);
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
      this.logService.error('[StorageService] Erro ao descriptografar dados do storage:', 'StorageService', e);
      return cipherText;
    }
  }

  public setItem(key: string, value: string): void {
    if (typeof window === 'undefined' || !window.localStorage) return;
    try {
      const encryptedValue = this.encrypt(value);
      window.localStorage.setItem(key, encryptedValue);
    } catch (e) {
      this.logService.error('[StorageService] Erro ao salvar no localStorage:', 'StorageService', e);
    }
  }

  public getItem(key: string): string | null {
    if (typeof window === 'undefined' || !window.localStorage) return null;
    try {
      const value = window.localStorage.getItem(key);
      if (!value) return null;
      return this.decrypt(value);
    } catch (e) {
      this.logService.error('[StorageService] Erro ao ler do localStorage:', 'StorageService', e);
      return null;
    }
  }

  public removeItem(key: string): void {
    if (typeof window === 'undefined' || !window.localStorage) return;
    window.localStorage.removeItem(key);
  }

  public clear(): void {
    if (typeof window === 'undefined' || !window.localStorage) return;
    window.localStorage.clear();
  }
}
