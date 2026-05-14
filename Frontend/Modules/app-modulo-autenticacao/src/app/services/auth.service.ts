import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LOG_CONFIG } from '@app/logs';
import { environment } from '../../environments/environment';

// Models
export interface LoginRequest {
    identificador: string;
    senha: string;
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
    email: string;
    codigoRecuperacao: string;
    novaSenha: string;
}

export interface AlterarSenhaRequest {
    email: string;
    senhaAtual: string;
    novaSenha: string;
}

@Injectable({
    providedIn: 'root'
})
export class AuthService {

    private urlBaseAutenticacao = environment.apiUrls.autenticacao;

    constructor(
        private httpClient: HttpClient,
        @Inject(LOG_CONFIG) private configuracaoLog: any
    ) { }

    login(solicitacaoLogin: LoginRequest): Observable<any> {
        const url = 'http://localhost:5006/app-api-autenticacao/autenticacao/login';
        console.log('[AuthService] Tentando login em:', url, solicitacaoLogin);
        return this.httpClient.post(url, solicitacaoLogin);
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

    alterarSenha(solicitacaoAlteracao: AlterarSenhaRequest): Observable<any> {
        return this.httpClient.post(`${this.urlBaseAutenticacao}/autenticacao/alterar-senha`, solicitacaoAlteracao);
    }

    public prepararNovaSessaoAposLogin(): void {
        if (typeof localStorage !== 'undefined') {
            // Limpa os IDs no localStorage. O Interceptor gerará novos na próxima requisição.
            localStorage.removeItem('SESSAO-ID');
            localStorage.removeItem('MESSAGE-ID');
        }
    }


}
