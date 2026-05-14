import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './features/login/login';
import { CadastroComponent } from './features/cadastro/cadastro';
import { RecuperarSenhaComponent } from './features/recuperar-senha/recuperar-senha';
import { RedefinirSenhaComponent } from './features/redefinir-senha/redefinir-senha';
import { AlterarSenhaComponent } from './features/alterar-senha/alterar-senha';

const routes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'cadastro', component: CadastroComponent },
    { path: 'recuperar-senha', component: RecuperarSenhaComponent },
    { path: 'redefinir-senha', component: RedefinirSenhaComponent },
    { path: 'alterar-senha', component: AlterarSenhaComponent },
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: '**', redirectTo: 'login' }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
