import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { LogService } from '@app/logs';

@Component({
  selector: 'app-recuperar-senha',
  templateUrl: './recuperar-senha.html',
  standalone: false
})
export class RecuperarSenhaComponent {
  email: string = '';
  loading: boolean = false;
  errorMessage: string = '';
  emailSent: boolean = false;

  constructor(
    private authService: AuthService,
    private logger: LogService
  ) { }

  onSubmit() {
    if (!this.email) return;

    this.loading = true;
    this.errorMessage = '';
    this.logger.info('Solicitando recuperação de senha', 'RecuperarSenhaComponent', { email: this.email });

    this.authService.solicitarResetSenha({ email: this.email, senhaAcesso: '' }).subscribe({
      next: () => {
        this.loading = false;
        this.emailSent = true;
        this.logger.info('Solicitação de reset de senha enviada', 'RecuperarSenhaComponent');
      },
      error: (err) => {
        this.loading = false;
        // Tenta pegar a mensagem de erro do backend (através do envelope de notificações)
        this.errorMessage = err.error?.statusProcessamento?.mensagemProcessamento || 
                            err.error?.mensagem || 
                            'Erro ao solicitar recuperação. Tente novamente mais tarde.';
        
        this.logger.error('Erro na recuperação de senha', 'RecuperarSenhaComponent', { error: err });
      }
    });
  }
}

