import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
    selector: 'app-cadastro',
    templateUrl: './cadastro.html',
    standalone: false
})
export class CadastroComponent {
    userData = {
        name: '',
        email: '',
        password: ''
    };
    loading = false;

    constructor(private router: Router) { }

    onRegister() {
        this.loading = true;
        console.log('Dados de cadastro:', this.userData);
        setTimeout(() => {
            this.loading = false;
            alert('Cadastro realizado com sucesso! Agora você pode fazer login.');
            this.router.navigate(['/login']);
        }, 1500);
    }
}
