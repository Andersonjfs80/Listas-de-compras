import { Component } from '@angular/core';
import { IonHeader, IonToolbar, IonTitle, IonContent, IonList, IonListHeader, IonItem, IonLabel, IonCheckbox, IonAvatar, IonFab, IonFabButton, IonIcon, IonFooter, IonGrid, IonRow, IonCol } from '@ionic/angular/standalone';
import { CommonModule } from '@angular/common';
import { addIcons } from 'ionicons';
import { add, calculator, cart } from 'ionicons/icons';

@Component({
    selector: 'app-listas',
    template: `
    <ion-header>
      <ion-toolbar color="primary">
        <ion-title>Minha lista</ion-title>
      </ion-toolbar>
    </ion-header>

    <ion-content>
      <div *ngFor="let cat of categorias">
        <ion-list-header [class.carrinho-header]="cat.destaque">
          <ion-label>{{cat.nome}}</ion-label>
        </ion-list-header>
        
        <ion-list>
          <ion-item *ngFor="let item of cat.itens">
            <ion-avatar slot="start">
              <img [src]="item.imagem" onerror="this.src='https://placehold.co/40x40?text=P'">
            </ion-avatar>
            <ion-label [class.riscado]="item.checked && cat.destaque">
              <h2>{{item.nome}}</h2>
              <p>{{item.descricao}}</p>
            </ion-label>
            <ion-checkbox slot="end" [(ngModel)]="item.checked"></ion-checkbox>
          </ion-item>
        </ion-list>
      </div>

      <ion-fab vertical="bottom" horizontal="end" slot="fixed" style="margin-bottom: 70px;">
        <ion-fab-button>
          <ion-icon name="add"></ion-icon>
        </ion-fab-button>
      </ion-fab>
    </ion-content>

    <ion-footer>
      <ion-toolbar color="success" style="--background: #a5d6a7;">
        <ion-grid>
          <ion-row>
            <ion-col class="ion-text-center">
              <div class="footer-stats">
                <ion-icon name="calculator" size="large"></ion-icon>
                <div>
                  <p>Total (5)</p>
                  <strong>R$ 5,00</strong>
                </div>
              </div>
            </ion-col>
            <ion-col class="ion-text-center">
              <div class="footer-stats">
                <ion-icon name="cart" size="large"></ion-icon>
                <div>
                  <p>Carrinho (1)</p>
                  <strong>R$ 1,00</strong>
                </div>
              </div>
            </ion-col>
          </ion-row>
        </ion-grid>
      </ion-toolbar>
    </ion-footer>
  `,
    styles: [`
    .carrinho-header { --background: #e8f5e9; color: #2e7d32; }
    .riscado { text-decoration: line-through; opacity: 0.6; }
    .footer-stats { display: flex; align-items: center; justify-content: center; gap: 8px; color: #333; }
    .footer-stats p { margin: 0; font-size: 0.7em; }
    .footer-stats strong { font-size: 1em; }
    ion-list-header { background: #f4f4f4; min-height: 40px; }
  `],
    standalone: true,
    imports: [IonHeader, IonToolbar, IonTitle, IonContent, IonList, IonListHeader, IonItem, IonLabel, IonCheckbox, IonAvatar, IonFab, IonFabButton, IonIcon, IonFooter, IonGrid, IonRow, IonCol, CommonModule],
})
export class ListasPage {
    constructor() {
        addIcons({ add, calculator, cart });
    }

    categorias = [
        {
            nome: 'Frutas, ovos e verduras',
            itens: [
                { nome: 'Banana', descricao: '1 un - R$ 1,00', imagem: 'assets/banana.png', checked: false },
                { nome: 'Limão', descricao: '1 un - R$ 1,00', imagem: 'assets/limao.png', checked: true }
            ]
        },
        {
            nome: 'Mercearia',
            itens: [
                { nome: 'Bolacha água e sal', descricao: '1 un - R$ 1,00', imagem: 'assets/bolacha1.png', checked: false }
            ]
        },
        {
            nome: 'Carrinho',
            destaque: true,
            itens: [
                { nome: 'Tomate', descricao: '1 un - R$ 1,00', imagem: 'assets/tomate.png', checked: true }
            ]
        }
    ];
}
