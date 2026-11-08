import { Component } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-produtos',
  templateUrl: './produtos.html',
  styleUrls: ['./produtos.scss'],
  standalone: true,
  imports: [CommonModule, FormsModule, MatListModule, MatIconModule, MatButtonModule, MatCardModule, DecimalPipe]
})
export class ProdutosComponent {
  categorias = [
    {
      nome: 'Hortifruti',
      produtos: [
        { nome: 'Maçã', preco: 5.99, unidade: 'kg', imagem: 'assets/maca.png' },
        { nome: 'Banana', preco: 3.50, unidade: 'kg', imagem: 'assets/banana.png' },
        { nome: 'Alface', preco: 2.00, unidade: 'un', imagem: 'assets/alface.png' }
      ]
    },
    {
      nome: 'Mercearia',
      produtos: [
        { nome: 'Arroz 5kg', preco: 22.90, unidade: 'pc', imagem: 'assets/arroz.png' },
        { nome: 'Feijão 1kg', preco: 8.50, unidade: 'pc', imagem: 'assets/feijao.png' },
        { nome: 'Macarrão', preco: 4.20, unidade: 'pc', imagem: 'assets/macarrao.png' }
      ]
    },
    {
      nome: 'Bebidas',
      produtos: [
        { nome: 'Refrigerante 2L', preco: 8.00, unidade: 'un', imagem: 'assets/refri.png' },
        { nome: 'Suco Natural', preco: 9.50, unidade: 'L', imagem: 'assets/suco.png' }
      ]
    }
  ];
}
