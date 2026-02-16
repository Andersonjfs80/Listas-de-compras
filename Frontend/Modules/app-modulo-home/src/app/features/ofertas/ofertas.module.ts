import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProdutosComponent } from './ofertas';

@NgModule({
    imports: [
        CommonModule,
        ProdutosComponent, // Standalone component imported here
        RouterModule.forChild([{ path: '', component: ProdutosComponent }])
    ],
    exports: [ProdutosComponent]
})
export class ProdutosListagemModule { }
