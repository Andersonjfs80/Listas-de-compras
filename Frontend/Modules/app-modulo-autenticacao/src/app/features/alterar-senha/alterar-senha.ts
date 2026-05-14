import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService, AlterarSenhaRequest } from '../../services/auth.service';
import { LogService, LoadingService } from '@app/logs';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
    selector: 'app-alterar-senha',
    templateUrl: './alterar-senha.html',
    standalone: false
})
export class AlterarSenhaComponent {
    senhaAtual: string = '';
    novaSenha: string = '';
    confirmarNovaSenha: string = '';
    
    hidePassword = true;
    errorMessage: string = '';
    loading = false;
    
    public loadingService = inject(LoadingService);

    constructor(
        private router: Router,
        private authService: AuthService,
        private logger: LogService,
        private snackBar: MatSnackBar
    ) { }

    onSubmit() {
        if (this.novaSenha !== this.confirmarNovaSenha) {
            this.errorMessage = 'As senhas não coincidem!';
            return;
        }

        const userJson = localStorage.getItem('user');
        if (!userJson) {
            this.errorMessage = 'Sessão inválida. Por favor, faça login novamente.';
            return;
        }

        const user = JSON.parse(userJson);
        this.errorMessage = '';
        this.logger.info('Iniciando alteração voluntária de senha', 'AlterarSenhaComponent');

        const request: AlterarSenhaRequest = {
            email: user.email,
            senhaAtual: this.senhaAtual,
            novaSenha: this.novaSenha
        };

        this.authService.alterarSenha(request)
            .subscribe({
                next: () => {
                    this.logger.info('Senha alterada com sucesso', 'AlterarSenhaComponent');
                    this.snackBar.open('Senha alterada com sucesso!', 'OK', { duration: 3000 });
                    this.router.navigate(['/login']);
                },
                error: (err) => {
                    const status = err.error?.statusProcessamento;
                    this.errorMessage = status?.mensagemProcessamento || 'Erro ao alterar senha. Verifique os dados e tente novamente.';
                    this.logger.error('Erro na alteração de senha', 'AlterarSenhaComponent', err);
                }
            });
    }
}
