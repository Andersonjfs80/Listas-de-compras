import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { HomeWrapperComponent } from './home-wrapper.component';

const routes: Routes = [
    {
        path: '',
        component: HomeWrapperComponent
    }
];

@NgModule({
    declarations: [HomeWrapperComponent],
    imports: [
        CommonModule,
        RouterModule.forChild(routes)
    ]
})
export class HomeModule { }
