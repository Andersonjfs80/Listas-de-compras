import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full',
  },
  {
    path: 'home',
    loadComponent: () => import('./home/home.page').then((m) => m.HomePage),
  },
  {
    path: 'listas',
    loadComponent: () => import('./listas/listas.page').then((m) => m.ListasPage),
  },
  {
    path: 'ofertas',
    loadComponent: () => import('./ofertas/ofertas.page').then((m) => m.OfertasPage),
  },
  {
    path: 'carrinho',
    loadComponent: () => import('./carrinho/carrinho.page').then((m) => m.CarrinhoPage),
  },
];
