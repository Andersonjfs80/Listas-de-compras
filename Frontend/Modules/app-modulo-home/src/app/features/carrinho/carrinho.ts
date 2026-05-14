import { Component, OnInit } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDividerModule } from '@angular/material/divider';
import { ListaComprasService, ItemListaDto } from '../../services/lista-compras.service';
import { forkJoin } from 'rxjs';

interface ItemCarrinho extends ItemListaDto {
  listaNome: string;
}

interface CategoriaCarrinho {
  nome: string;
  itens: ItemCarrinho[];
}

@Component({
  selector: 'app-carrinho',
  templateUrl: './carrinho.html',
  styleUrls: ['./carrinho.scss'],
  standalone: true,
  imports: [
    CommonModule,
    CurrencyPipe,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDividerModule
  ]
})
export class CarrinhoComponent implements OnInit {

  categorias: CategoriaCarrinho[] = [];
  totalItens = 0;
  totalEstimado = 0;
  carregando = false;
  erro: string | null = null;

  constructor(
    private listaComprasService: ListaComprasService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.carregarCarrinho();
  }

  carregarCarrinho(): void {
    this.carregando = true;
    this.erro = null;

    this.listaComprasService.obterListas().subscribe({
      next: (response) => {
        const itensMarcados: ItemCarrinho[] = [];

        for (const lista of (response.listas || [])) {
          for (const item of (lista.itens || [])) {
            if (item.marcado) {
              itensMarcados.push({ ...item, listaNome: lista.nome });
            }
          }
        }

        this.agruparPorCategoria(itensMarcados);
        this.calcularTotal(itensMarcados);
        this.carregando = false;
      },
      error: () => {
        this.erro = 'Não foi possível carregar o carrinho. Tente novamente.';
        this.carregando = false;
      }
    });
  }

  private agruparPorCategoria(itens: ItemCarrinho[]): void {
    const mapa = new Map<string, ItemCarrinho[]>();
    for (const item of itens) {
      const cat = item.categoriaNome || 'Outros';
      if (!mapa.has(cat)) mapa.set(cat, []);
      mapa.get(cat)!.push(item);
    }
    this.categorias = [];
    mapa.forEach((itensGrupo, nome) => {
      this.categorias.push({ nome, itens: itensGrupo });
    });
    this.totalItens = itens.length;
  }

  private calcularTotal(itens: ItemCarrinho[]): void {
    // Usa precoCompra se disponível, senão não soma
    this.totalEstimado = itens.reduce((acc, item) => {
      const preco = (item as any).precoCompra || 0;
      return acc + (preco * item.quantidade);
    }, 0);
  }

  limparCarrinho(): void {
    this.snackBar.open('Funcionalidade em breve!', 'Fechar', { duration: 2000 });
  }
}
