import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService, CadastrarSenhaRequest } from '../../services/auth.service';
import { LogService, LoadingService } from '@app/logs';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'app-redefinir-senha',
    templateUrl: './redefinir-senha.html',
    standalone: false
})
export class RedefinirSenhaComponent implements OnInit {
    passwordData = {
        password: '',
        confirmPassword: ''
    };
    email: string = '';
    codigoRecuperacao: string = '';
    hidePassword = true;
    errorMessage: string = '';
    loading = false;
    
    public loadingService = inject(LoadingService);

    constructor(
        private route: ActivatedRoute, 
        private router: Router,
        private authService: AuthService,
        private logger: LogService
    ) { }

    ngOnInit() {
        this.codigoRecuperacao = this.route.snapshot.queryParamMap.get('token') || this.route.snapshot.queryParamMap.get('codigo') || '';
        this.email = this.route.snapshot.queryParamMap.get('email') || '';
        
        if (!this.codigoRecuperacao) {
            this.errorMessage = 'Código de recuperação não encontrado ou inválido.';
            this.logger.warn('Acesso à redefinição de senha sem código', 'RedefinirSenhaComponent');
        }
    }

    onSubmit() {
        if (!this.codigoRecuperacao) return;

        if (this.passwordData.password !== this.passwordData.confirmPassword) {
            this.errorMessage = 'As senhas não coincidem!';
            return;
        }

        this.errorMessage = '';
        this.loading = true;
        this.logger.info('Iniciando redefinição de senha', 'RedefinirSenhaComponent');

        const request: CadastrarSenhaRequest = {
            email: this.email,
            codigoRecuperacao: this.codigoRecuperacao,
            novaSenha: this.passwordData.password
        };

        this.authService.redefinirSenha(request)
            .pipe(finalize(() => this.loading = false))
            .subscribe({
                next: () => {
                    this.logger.info('Senha redefinida com sucesso', 'RedefinirSenhaComponent');
                    this.router.navigate(['/login'], { queryParams: { success: 'true' } });
                },
                error: (err) => {
                    const status = err.error?.statusProcessamento;
                    this.errorMessage = status?.mensagemProcessamento || 'Erro ao redefinir senha. O código pode ter expirado ou já foi utilizado.';
                    this.logger.error('Erro na redefinição de senha', 'RedefinirSenhaComponent', err);
                }
            });
    }
}
