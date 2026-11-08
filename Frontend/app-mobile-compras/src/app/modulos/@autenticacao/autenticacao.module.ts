import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AutenticacaoWrapperComponent } from './autenticacao-wrapper.component';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { path: '', component: AutenticacaoWrapperComponent }
];

@NgModule({
  declarations: [AutenticacaoWrapperComponent],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ]
})
export class AutenticacaoModule { }
