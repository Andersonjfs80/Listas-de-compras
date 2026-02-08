import { Component } from '@angular/core';
import { IonApp, IonRouterOutlet, IonTabs, IonTabBar, IonTabButton, IonIcon, IonLabel } from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { homeOutline, listOutline, pricetagOutline, cartOutline } from 'ionicons/icons';

@Component({
  selector: 'app-root',
  template: `
    <ion-app>
      <ion-tabs>
        <ion-router-outlet></ion-router-outlet>
        
        <ion-tab-bar slot="bottom">
          <ion-tab-button tab="home">
            <ion-icon name="home-outline"></ion-icon>
            <ion-label>Início</ion-label>
          </ion-tab-button>

          <ion-tab-button tab="listas">
            <ion-icon name="list-outline"></ion-icon>
            <ion-label>Listas</ion-label>
          </ion-tab-button>

          <ion-tab-button tab="ofertas">
            <ion-icon name="pricetag-outline"></ion-icon>
            <ion-label>Ofertas</ion-label>
          </ion-tab-button>

          <ion-tab-button tab="carrinho">
            <ion-icon name="cart-outline"></ion-icon>
            <ion-label>Carrinho</ion-label>
          </ion-tab-button>
        </ion-tab-bar>
      </ion-tabs>
    </ion-app>
  `,
  standalone: true,
  imports: [IonApp, IonRouterOutlet, IonTabs, IonTabBar, IonTabButton, IonIcon, IonLabel],
})
export class AppComponent {
  constructor() {
    addIcons({ homeOutline, listOutline, pricetagOutline, cartOutline });
  }
}
