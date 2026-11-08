import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LOG_CONFIG, LogService, StorageService } from '@app/logs';
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
        private logService: LogService,
        private storageService: StorageService,
        @Inject(LOG_CONFIG) private configuracaoLog: any
    ) { }

    login(solicitacaoLogin: LoginRequest): Observable<any> {
        const url = 'http://localhost:5006/app-api-autenticacao/autenticacao/login';
        this.logService.info('[AuthService] Tentando login no BFF Gateway de Autenticação.', 'AuthService', { url });
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
        // Limpa os IDs no localStorage. O Interceptor gerará novos na próxima requisição.
        this.storageService.removeItem('SESSAO-ID');
        this.storageService.removeItem('MESSAGE-ID');
    }
}
