import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { StorageService } from '@app/logs';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private apiUrl = environment.apiUrls.autenticacao; // Centralizado no environment

    constructor(
        private http: HttpClient,
        private storageService: StorageService
    ) { }

    logout(): void {
        this.storageService.removeItem('token');
        this.storageService.removeItem('user');
    }

    getToken(): string | null {
        const token = this.storageService.getItem('token');
        if (!token || token === 'null' || token === 'undefined' || token.trim() === '') {
            return null;
        }
        return token;
    }

    isAuthenticated(): boolean {
        const token = this.getToken();
        if (!token) return false;
        
        const partes = token.split('.').length;
        // Aceita tanto JWT convencional (3 partes) quanto JWE criptografado (5 partes)
        if (partes !== 3 && partes !== 5) {
            return false;
        }

        // Validação defensiva de expiração para JWT convencionais (3 partes)
        try {
            if (partes === 3) {
                const payloadBase64 = token.split('.')[1];
                const payloadJson = atob(payloadBase64.replace(/-/g, '+').replace(/_/g, '/'));
                const payload = JSON.parse(payloadJson);
                if (payload.exp) {
                    const expiraEmMilissegundos = payload.exp * 1000;
                    if (Date.now() >= expiraEmMilissegundos) {
                        return false; // Token expirado
                    }
                }
            }
        } catch (e) {
            return false; // Formato inválido/corrompido
        }

        return true;
    }
}
