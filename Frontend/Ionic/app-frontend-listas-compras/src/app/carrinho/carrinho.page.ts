import { Component } from '@angular/core';
import { IonHeader, IonToolbar, IonTitle, IonContent } from '@ionic/angular/standalone';

@Component({
    selector: 'app-carrinho',
    template: `
    <ion-header>
      <ion-toolbar color="primary">
        <ion-title>Carrinho</ion-title>
      </ion-toolbar>
    </ion-header>
    <ion-content class="ion-padding">
      <p>Página do Carrinho em construção...</p>
    </ion-content>
  `,
    standalone: true,
    imports: [IonHeader, IonToolbar, IonTitle, IonContent],
})
export class CarrinhoPage { }
