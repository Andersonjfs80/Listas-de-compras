import { Component } from '@angular/core';
import { IonHeader, IonToolbar, IonTitle, IonContent } from '@ionic/angular/standalone';

@Component({
    selector: 'app-ofertas',
    template: `
    <ion-header>
      <ion-toolbar color="primary">
        <ion-title>Ofertas</ion-title>
      </ion-toolbar>
    </ion-header>
    <ion-content class="ion-padding">
      <p>Página de Ofertas em construção...</p>
    </ion-content>
  `,
    standalone: true,
    imports: [IonHeader, IonToolbar, IonTitle, IonContent],
})
export class OfertasPage { }
