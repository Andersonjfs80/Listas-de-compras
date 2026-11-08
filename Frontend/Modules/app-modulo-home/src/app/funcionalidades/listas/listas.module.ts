import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ListasComponent } from './listas';

@NgModule({
    imports: [
        CommonModule,
        ListasComponent, // Standalone component imported here
        RouterModule.forChild([{ path: '', component: ListasComponent }])
    ],
    exports: [ListasComponent]
})
export class ListasListagemModule { }
