import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService, LoginRequest } from '../../services/auth.service';
import { LogService } from '@app/logs';

@Component({
    selector: 'app-login',
    templateUrl: './login.html',
    standalone: false
})
export class LoginComponent {
    loginData: LoginRequest = {
        identificador: '',
        senhaAcesso: ''
    };
    hidePassword = true;
    loading = false;
    errorMessage = '';
    lembrarMe = false;

    constructor(
        private router: Router,
        private authService: AuthService,
        private logger: LogService
    ) { }

    ngOnInit() {
        const savedIdentifier = localStorage.getItem('savedIdentifier');
        if (savedIdentifier) {
            this.loginData.identificador = savedIdentifier;
            this.lembrarMe = true;
        }
    }

    onLogin() {
        if (!this.loginData.identificador || !this.loginData.senhaAcesso) {
            return;
        }

        this.loading = true;
        this.errorMessage = '';
        this.logger.info('Iniciando tentativa de login', 'LoginComponent', { identificador: this.loginData.identificador });

        this.authService.login(this.loginData).subscribe({
            next: (response) => {
                this.loading = false;
                this.logger.info('Login realizado com sucesso', 'LoginComponent', response);

                if (this.lembrarMe) {
                    localStorage.setItem('savedIdentifier', this.loginData.identificador);
                } else {
                    localStorage.removeItem('savedIdentifier');
                }

                // Salvar token se necessário (localStorage.setItem('token', response.token))

                // Redireciona para o módulo de Home (Porta 4202)
                window.location.href = '/home';
            },
            error: (error) => {
                this.loading = false;
                this.logger.error('Erro no login', 'LoginComponent', error);
                this.errorMessage = 'Usuário ou senha inválidos. Tente novamente.';
            }
        });
    }
}
