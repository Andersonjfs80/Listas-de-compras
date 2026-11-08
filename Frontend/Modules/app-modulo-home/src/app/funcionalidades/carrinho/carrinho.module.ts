import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CarrinhoComponent } from './carrinho';

@NgModule({
    imports: [
        CommonModule,
        CarrinhoComponent, // Standalone component imported here
        RouterModule.forChild([{ path: '', component: CarrinhoComponent }])
    ],
    exports: [CarrinhoComponent]
})
export class CarrinhoModule { }
