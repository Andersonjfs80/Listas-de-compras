import { Component, ChangeDetectorRef, NgZone, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService, LoginRequest } from '../../services/auth.service';
import { LogService, LoadingService } from '@app/logs';
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
        if (typeof localStorage !== 'undefined') {
            const savedIdentifier = localStorage.getItem('savedIdentifier');
            if (savedIdentifier) {
                this.loginData.identificador = savedIdentifier;
                this.lembrarMe = true;
            }
        }
    }

    onLogin() {
        if (!this.loginData.identificador || !this.loginData.senha) {
            return;
        }

        this.errorMessage = '';
        this.logger.info('Iniciando tentativa de login', 'LoginComponent', { identificador: this.loginData.identificador });

        // O interceptor já vai ativar o loadingService.setLoading(true)
        this.authService.login(this.loginData).pipe(
            // O finalize garante o "finally" (destrava a tela)
            finalize(() => {
                this.cdr.detectChanges();
            })
        ).subscribe({
            next: (response) => {
                this.logger.info('Login realizado com sucesso', 'LoginComponent');
                
                if (typeof localStorage !== 'undefined') {
                    if (this.lembrarMe) {
                        localStorage.setItem('savedIdentifier', this.loginData.identificador);
                    } else {
                        localStorage.removeItem('savedIdentifier');
                    }

                    if (response.token) {
                        localStorage.setItem('token', response.token);
                    }

                    if (response.usuario) {
                        localStorage.setItem('user', JSON.stringify(response.usuario));
                    }
                }

                this.authService.prepararNovaSessaoAposLogin();
                
                // Tratar Avisos (Ex: Senha prestes a expirar)
                const status = response.statusProcessamento;
                if (status?.codigoProcessamento === 'AUTH004') {
                    this.snackBar.open(status.mensagemProcessamento, 'Entendi', { duration: 10000 });
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

