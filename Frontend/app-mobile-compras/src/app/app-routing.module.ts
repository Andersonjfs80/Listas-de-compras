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
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

