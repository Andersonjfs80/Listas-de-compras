import { Component } from '@angular/core';
import { IonHeader, IonToolbar, IonTitle, IonContent, IonCard, IonCardContent, IonLabel, IonBadge, IonIcon, IonButton } from '@ionic/angular/standalone';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  template: `
    <ion-header [translucent]="true">
      <ion-toolbar color="primary">
        <ion-title>Início</ion-title>
      </ion-toolbar>
    </ion-header>

    <ion-content [fullscreen]="true">
      <div class="section">
        <h3>Mais Comprados</h3>
        <div class="horizontal-scroll">
          <ion-card *ngFor="let item of maisComprados">
            <img [src]="item.imagem" onerror="this.src='https://placehold.co/100x100?text=Produto'">
            <ion-card-content>
              <ion-label>
                <h2>{{item.nome}}</h2>
                <p>R$ {{item.preco | number:'1.2-2'}}</p>
              </ion-label>
            </ion-card-content>
          </ion-card>
        </div>
      </div>

      <div class="section">
        <h3>Ofertas Imperdíveis 🏷️</h3>
        <div class="horizontal-scroll">
          <ion-card *ngFor="let item of ultimasOfertas" class="oferta-card">
            <ion-badge color="danger">OFERTA</ion-badge>
            <img [src]="item.imagem" onerror="this.src='https://placehold.co/100x100?text=Oferta'">
            <ion-card-content>
              <ion-label>
                <h2>{{item.nome}}</h2>
                <p class="old-price">R$ {{item.precoAnterior | number:'1.2-2'}}</p>
                <p class="new-price">R$ {{item.preco | number:'1.2-2'}}</p>
              </ion-label>
            </ion-card-content>
          </ion-card>
        </div>
      </div>
    </ion-content>
  `,
  styles: [`
    .section { padding: 16px; }
    h3 { margin-top: 0; font-weight: bold; }
    .horizontal-scroll {
      display: flex;
      overflow-x: auto;
      gap: 8px;
      padding-bottom: 8px;
    }
    ion-card {
      min-width: 150px;
      margin: 0;
      img { height: 100px; width: 100%; object-fit: contain; padding: 10px; }
    }
    .old-price { text-decoration: line-through; color: #999; font-size: 0.8em; }
    .new-price { font-weight: bold; color: var(--ion-color-success); font-size: 1.1em; }
    .oferta-card { position: relative; }
    ion-badge { position: absolute; top: 10px; right: 10px; }
  `],
  standalone: true,
  imports: [IonHeader, IonToolbar, IonTitle, IonContent, IonCard, IonCardContent, IonLabel, IonBadge, IonIcon, IonButton, CommonModule],
})
export class HomePage {
  maisComprados = [
    { nome: 'Banana', preco: 1.00, imagem: 'assets/banana.png' },
    { nome: 'Limão', preco: 1.00, imagem: 'assets/limao.png' },
    { nome: 'Arroz 5kg', preco: 25.90, imagem: 'assets/arroz.png' }
  ];

  ultimasOfertas = [
    { nome: 'Cerveja Pack', preco: 35.00, precoAnterior: 42.00, imagem: 'assets/cerveja.png' },
    { nome: 'Sabão em Pó', preco: 15.00, precoAnterior: 19.00, imagem: 'assets/sabao.png' }
  ];
}
