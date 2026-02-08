import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.html',
  styleUrls: ['./home.scss']
})
export class HomeComponent {
  maisComprados = [
    { nome: 'Banana', preco: 1.00, imagem: 'assets/banana.png', oferta: false },
    { nome: 'Limão', preco: 1.00, imagem: 'assets/limao.png', oferta: false },
    { nome: 'Arroz 5kg', preco: 25.90, imagem: 'assets/arroz.png', oferta: false },
    { nome: 'Leite', preco: 4.50, imagem: 'assets/leite.png', oferta: true }
  ];

  ultimasOfertas = [
    { nome: 'Cerveja Pack', preco: 35.00, precoAnterior: 42.00, imagem: 'assets/cerveja.png', oferta: true },
    { nome: 'Pão de Forma', preco: 6.50, precoAnterior: 8.90, imagem: 'assets/pao.png', oferta: true },
    { nome: 'Sabão em Pó', preco: 15.00, precoAnterior: 19.00, imagem: 'assets/sabao.png', oferta: true }
  ];
}
