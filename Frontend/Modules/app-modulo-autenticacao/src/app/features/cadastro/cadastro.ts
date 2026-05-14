import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService, CadastroRequest } from '../../services/auth.service';
import { LogService } from '@app/logs';

@Component({
    selector: 'app-cadastro',
    templateUrl: './cadastro.html',
    standalone: false
})
export class CadastroComponent {
    userData: CadastroRequest = {
        nome: '',
        email: '',
        senhaAcesso: ''
    };
    loading = false;
    errorMessage = '';

    constructor(
        private router: Router,
        private authService: AuthService,
        private logger: LogService
    ) { }

    onRegister() {
        if (!this.userData.nome || !this.userData.email || !this.userData.senhaAcesso) {
            return;
        }

        this.loading = true;
        this.errorMessage = '';
        this.logger.info('Iniciando cadastro de usuário', 'CadastroComponent', { email: this.userData.email });

        this.authService.cadastrar(this.userData).subscribe({
            next: () => {
                this.logger.info('Usuário cadastrado com sucesso', 'CadastroComponent');
                this.router.navigate(['/login']);
            },
            error: (err) => {
                this.loading = false;
                this.errorMessage = 'Erro ao realizar cadastro. Tente novamente mais tarde.';
                this.logger.error('Erro no cadastro', 'CadastroComponent', { error: err });
            }
        });
    }
}

