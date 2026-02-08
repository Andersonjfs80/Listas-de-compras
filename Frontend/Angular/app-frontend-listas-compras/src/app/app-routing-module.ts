import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './features/home/home';
import { ListasComponent } from './features/listas/listas';
import { OfertasComponent } from './features/ofertas/ofertas';
import { CarrinhoComponent } from './features/carrinho/carrinho';

const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  { path: 'listas', component: ListasComponent },
  { path: 'ofertas', component: OfertasComponent },
  { path: 'carrinho', component: CarrinhoComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

