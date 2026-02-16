import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private apiUrl = 'https://localhost/app-api-autenticacao'; // Desenvolvimento (Nome Completo)

    constructor(private http: HttpClient) { }

    logout(): void {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
    }

    getToken(): string | null {
        return localStorage.getItem('token');
    }

    isAuthenticated(): boolean {
        return !!this.getToken();
    }
}
