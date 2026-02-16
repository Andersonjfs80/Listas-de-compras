import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

const routes: Routes = [
    {
        path: '',
        redirectTo: 'home',
        pathMatch: 'full'
    },
    {
        path: 'home',
        loadComponent: () => import('./features/home/home').then(m => m.HomeComponent),
        canActivate: [authGuard]
    },
    {
        path: 'listas',
        loadComponent: () => import('./features/listas/listas').then(m => m.ListasComponent),
        canActivate: [authGuard]
    },
    {
        path: 'carrinho',
        loadComponent: () => import('./features/carrinho/carrinho').then(m => m.CarrinhoComponent),
        canActivate: [authGuard]
    },
    {
        path: 'produtos',
        loadComponent: () => import('./features/ofertas/ofertas').then(m => m.ProdutosComponent),
        canActivate: [authGuard]
    }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
