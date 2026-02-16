import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LOG_CONFIG } from '@app/logs';

// Models
export interface LoginRequest {
    identificador: string;
    senhaAcesso: string;
}

export interface CadastroRequest {
    nome: string;
    email: string;
    senhaAcesso: string;
}

export interface ResetSenhaRequest {
    email: string;
    senhaAcesso: string;
}

export interface CadastrarSenhaRequest {
    token: string;
    senhaAcesso: string;
    confirmacaoSenha: string;
}

@Injectable({
    providedIn: 'root'
})
export class AuthService {

    private apiUrl: string;

    constructor(
        private http: HttpClient,
        @Inject(LOG_CONFIG) private config: any
    ) {
        this.apiUrl = `${this.config.apiUrl}/api/auth`;
    }

    login(dados: LoginRequest): Observable<any> {
        return this.http.post(`${this.apiUrl}/login`, dados);
    }

    cadastrar(dados: CadastroRequest): Observable<any> {
        return this.http.post(`${this.apiUrl}/cadastrar`, dados);
    }

    solicitarResetSenha(dados: ResetSenhaRequest): Observable<any> {
        return this.http.post(`${this.apiUrl}/resetar-senha`, dados);
    }

    redefinirSenha(dados: CadastrarSenhaRequest): Observable<any> {
        return this.http.post(`${this.apiUrl}/cadastrar-senha`, dados);
    }
}
