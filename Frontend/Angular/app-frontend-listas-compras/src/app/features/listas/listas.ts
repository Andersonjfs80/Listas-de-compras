import { Component } from '@angular/core';

@Component({
  selector: 'app-listas',
  templateUrl: './listas.html',
  styleUrls: ['./listas.scss']
})
export class ListasComponent {
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
        { nome: 'Bolacha água e sal', descricao: '1 un - R$ 1,00', imagem: 'assets/bolacha1.png', checked: false },
        { nome: 'Bolacha recheada', descricao: '1 un - R$ 1,00', imagem: 'assets/bolacha2.png', checked: false }
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

  totalItens = 5;
  valorTotal = 5.00;
  itensNoCarrinho = 1;
  valorNoCarrinho = 1.00;
}
