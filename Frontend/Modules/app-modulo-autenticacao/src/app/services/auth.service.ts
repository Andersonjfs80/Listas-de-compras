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

    private urlBaseAutenticacao: string;

    constructor(
        private httpClient: HttpClient,
        @Inject(LOG_CONFIG) private configuracaoLog: any
    ) {
        // A URL base já vem do environment via LOG_CONFIG (ex: http://localhost/app-api-autenticacao)
        this.urlBaseAutenticacao = this.configuracaoLog.apiUrl;
    }

    login(solicitacaoLogin: LoginRequest): Observable<any> {
        // Agora sem cabeçalhos manuais! O Interceptor injeta tudo sozinho.
        return this.httpClient.post(`${this.urlBaseAutenticacao}/autenticacao/login`, solicitacaoLogin);
    }

    cadastrar(solicitacaoCadastro: CadastroRequest): Observable<any> {
        return this.httpClient.post(`${this.urlBaseAutenticacao}/autenticacao/cadastrar`, solicitacaoCadastro);
    }

    solicitarResetSenha(solicitacaoReset: ResetSenhaRequest): Observable<any> {
        return this.httpClient.post(`${this.urlBaseAutenticacao}/autenticacao/resetar-senha`, solicitacaoReset);
    }

    redefinirSenha(solicitacaoRedefinicao: CadastrarSenhaRequest): Observable<any> {
        return this.httpClient.post(`${this.urlBaseAutenticacao}/autenticacao/cadastrar-senha`, solicitacaoRedefinicao);
    }

    public prepararNovaSessaoAposLogin(): void {
        // Limpa os IDs no localStorage. O Interceptor gerará novos na próxima requisição.
        localStorage.removeItem('SESSAO-ID');
        localStorage.removeItem('MESSAGE-ID');
    }
}
