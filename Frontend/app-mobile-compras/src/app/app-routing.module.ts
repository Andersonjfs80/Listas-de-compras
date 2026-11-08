import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Welcome } from './modulos/welcome/welcome';

const routes: Routes = [
  {
    path: '',
    redirectTo: '/home',
    pathMatch: 'full'
  },
  {
    path: 'home',
    loadChildren: () => import('./modulos/@home/home.module').then(m => m.HomeModule),
    data: { url: 'http://localhost:4202/home' }
  },
  {
    path: 'listas',
    loadChildren: () => import('./modulos/@home/home.module').then(m => m.HomeModule),
    data: { url: 'http://localhost:4202/listas' }
  },
  {
    path: 'ofertas',
    loadChildren: () => import('./modulos/@home/home.module').then(m => m.HomeModule),
    data: { url: 'http://localhost:4202/ofertas' }
  },
  {
    path: 'carrinho',
    loadChildren: () => import('./modulos/@home/home.module').then(m => m.HomeModule),
    data: { url: 'http://localhost:4202/carrinho' }
  },
  {
    path: 'welcome',
    component: Welcome
  },
  {
    path: 'servicos',
    loadChildren: () => import('./modulos/@autenticacao/autenticacao.module').then(m => m.AutenticacaoModule),
    data: { url: 'http://localhost:4204/login' }
  },
  {
    path: 'cadastro',
    loadChildren: () => import('./modulos/@autenticacao/autenticacao.module').then(m => m.AutenticacaoModule),
    data: { url: 'http://localhost:4204/cadastro' }
  },
  {
    path: 'alterar-senha',
    loadChildren: () => import('./modulos/@autenticacao/autenticacao.module').then(m => m.AutenticacaoModule),
    data: { url: 'http://localhost:4204/alterar-senha' }
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

