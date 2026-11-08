import { Component, ChangeDetectorRef, NgZone, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService, LoginRequest } from '../../services/auth.service';
import { LogService, LoadingService, CryptoService, StorageService } from '@app/logs';
import { finalize } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
    selector: 'app-login',
    templateUrl: './login.html',
    standalone: false
})
export class LoginComponent {
    loginData: LoginRequest = {
        identificador: '',
        senha: ''
    };
    hidePassword = true;
    errorMessage = '';
    lembrarMe = false;
    loading = false;

    // Usando o serviço global injetado
    public loadingService = inject(LoadingService);
    private cryptoService = inject(CryptoService);
    private storageService = inject(StorageService);

    togglePassword() {
        this.hidePassword = !this.hidePassword;
    }

    constructor(
        private router: Router,
        private authService: AuthService,
        private logger: LogService,
        private snackBar: MatSnackBar,
        private cdr: ChangeDetectorRef,
        private zone: NgZone
    ) { }

    ngOnInit() {
        const savedIdentifier = this.storageService.getItem('savedIdentifier');
        if (savedIdentifier) {
            this.loginData.identificador = savedIdentifier;
            this.lembrarMe = true;
        }
    }

    onLogin() {
        if (!this.loginData.identificador || !this.loginData.senha) {
            return;
        }

        this.errorMessage = '';
        this.logger.info('Iniciando tentativa de login', 'LoginComponent', { identificador: this.loginData.identificador });

        // Aplica Criptografia de Campo na senha antes do envio (Field Encryption)
        const requestData: LoginRequest = {
            identificador: this.loginData.identificador,
            senha: this.cryptoService.encryptFixed(this.loginData.senha)
        };

        // O interceptor já vai ativar o loadingService.setLoading(true)
        this.authService.login(requestData).pipe(
            // O finalize garante o "finally" (destrava a tela)
            finalize(() => {
                this.cdr.detectChanges();
            })
        ).subscribe({
            next: (response) => {
                this.logger.info('Login realizado com sucesso', 'LoginComponent');
                if (this.lembrarMe) {
                    this.storageService.setItem('savedIdentifier', this.loginData.identificador);
                } else {
                    this.storageService.removeItem('savedIdentifier');
                }

                // Extração robusta: cobre JSON descriptografado, formato CQRS (.data) ou JWE bruto
                let tokenReal = response.token || response?.data?.token;
                let usuarioReal = response.usuario || response?.data?.usuario;

                // Se não achou .token, mas .data é uma string, o interceptor pode ter falhado ou o token é o próprio data
                if (!tokenReal && typeof response?.data === 'string') {
                    tokenReal = response.data;
                }

                if (tokenReal) {
                    this.storageService.setItem('token', tokenReal);
                }

                if (usuarioReal) {
                    this.storageService.setItem('user', JSON.stringify(usuarioReal));
                }

                this.authService.prepararNovaSessaoAposLogin();

                // Tratar Avisos (Ex: Senha prestes a expirar)
                const status = response.statusProcessamento || response?.data?.statusProcessamento;
                if (status?.codigoProcessamento === 'AUTH004') {
                    this.snackBar.open(status.mensagemProcessamento, 'Entendi', { duration: 10000 });
                }

                // Tratar Integração via Iframe (SSO MFE)
                if (window.parent !== window) {
                    this.logger.info('Autenticação concluída dentro de Iframe. Enviando mensagem para o componente Pai.', 'LoginComponent');

                    const tokenParaPost = response.token || response?.data?.token;
                    const usuarioParaPost = response.usuario || response?.data?.usuario;

                    window.parent.postMessage({
                        type: 'AUTH_SUCCESS',
                        token: tokenParaPost,
                        usuario: usuarioParaPost
                    }, '*');
                    return;
                }

                window.location.href = '/home';
            },
            error: (err) => {
                const status = err.error?.statusProcessamento;

                if (status?.codigoProcessamento === 'AUTH003') {
                    // Senha Expirada - Redirecionar para troca
                    this.errorMessage = status.mensagemProcessamento;
                    this.snackBar.open('Sua senha expirou. Você será redirecionado para cadastrar uma nova.', 'OK', { duration: 5000 });

                    setTimeout(() => {
                        this.router.navigate(['/redefinir-senha'], { queryParams: { email: this.loginData.identificador } });
                    }, 3000);
                } else {
                    this.errorMessage = status?.mensagemProcessamento || 'Usuário ou senha inválidos. Tente novamente.';
                }

                this.logger.error('Erro ao realizar login', 'LoginComponent', { error: err });
                this.cdr.detectChanges();
            }
        });
    }
}

