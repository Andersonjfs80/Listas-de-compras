import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

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
    token: string = '';
    loading: boolean = false;
    hidePassword = true;

    constructor(private route: ActivatedRoute, private router: Router) { }

    ngOnInit() {
        this.token = this.route.snapshot.queryParamMap.get('token') || '';
    }

    onSubmit() {
        if (this.passwordData.password !== this.passwordData.confirmPassword) {
            alert('As senhas não coincidem!');
            return;
        }

        this.loading = true;
        setTimeout(() => {
            this.loading = false;
            alert('Senha redefinida com sucesso!');
            this.router.navigate(['/login']);
        }, 2000);
    }
}
