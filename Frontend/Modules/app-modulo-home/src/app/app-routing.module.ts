import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

const routes: Routes = [
    {
        path: 'login',
        loadComponent: () => import('./funcionalidades/login/login').then(m => m.LoginWrapperComponent)
    },
    {
        path: '',
        loadComponent: () => import('./funcionalidades/home/home').then(m => m.HomeComponent),
        canActivate: [authGuard]
    },
    {
        path: 'home',
        loadComponent: () => import('./funcionalidades/home/home').then(m => m.HomeComponent),
        canActivate: [authGuard]
    },
    {
        path: 'listas',
        loadComponent: () => import('./funcionalidades/listas/listas').then(m => m.ListasComponent),
        canActivate: [authGuard]
    },
    {
        path: 'carrinho',
        loadComponent: () => import('./funcionalidades/carrinho/carrinho').then(m => m.CarrinhoComponent),
        canActivate: [authGuard]
    },
    {
        path: 'produtos',
        loadComponent: () => import('./funcionalidades/ofertas/ofertas').then(m => m.ProdutosComponent),
        canActivate: [authGuard]
    },
    {
        path: '**',
        redirectTo: ''
    }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
