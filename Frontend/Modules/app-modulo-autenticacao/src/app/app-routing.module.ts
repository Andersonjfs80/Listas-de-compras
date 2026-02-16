import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './features/login/login';
import { CadastroComponent } from './features/cadastro/cadastro';
import { RecuperarSenhaComponent } from './features/recuperar-senha/recuperar-senha';
import { RedefinirSenhaComponent } from './features/redefinir-senha/redefinir-senha';

const routes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'cadastro', component: CadastroComponent },
    { path: 'recuperar-senha', component: RecuperarSenhaComponent },
    { path: 'redefinir-senha', component: RedefinirSenhaComponent },
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: '**', redirectTo: 'login' }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
