import { Component } from '@angular/core';

@Component({
  selector: 'app-recuperar-senha',
  templateUrl: './recuperar-senha.html',
  standalone: false
})
export class RecuperarSenhaComponent {
  email: string = '';
  loading: boolean = false;
  emailSent: boolean = false;

  onSubmit() {
    this.loading = true;
    console.log('Solicitando reset de senha para:', this.email);

    setTimeout(() => {
      this.loading = false;
      this.emailSent = true;
    }, 1500);
  }
}
