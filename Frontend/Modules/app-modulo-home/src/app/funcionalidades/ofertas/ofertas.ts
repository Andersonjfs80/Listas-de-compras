import { Component, OnInit } from '@angular/core';
import { CommonModule, DecimalPipe, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { ProdutoService, Produto, TipoEstabelecimento } from '../../services/produto.service';
import { LogService } from '@app/logs';

@Component({
  selector: 'app-produtos',
  templateUrl: './ofertas.html', // Mantendo o nome do arquivo original por enquanto
  styleUrls: ['./ofertas.scss'], // Mantendo o nome do arquivo original
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatSelectModule,
    DecimalPipe,
    CurrencyPipe
  ]
})
export class ProdutosComponent implements OnInit {
  produtos: Produto[] = [];
  tiposEstabelecimento: TipoEstabelecimento[] = [];
  filtroTipoId: string | undefined;

  carregando = false;

  constructor(private produtoService: ProdutoService, private logService: LogService) { }

  ngOnInit(): void {
    this.carregarTipos();
    this.carregarProdutos();
  }

  carregarTipos(): void {
    this.produtoService.listarTiposEstabelecimento().subscribe({
      next: (dados) => this.tiposEstabelecimento = dados,
      error: (erro) => this.logService.error('Erro ao carregar tipos', 'ProdutosComponent', erro)
    });
  }

  carregarProdutos(): void {
    this.carregando = true;
    this.produtoService.listarProdutos(1, 20, this.filtroTipoId).subscribe({
      next: (resultado) => {
        this.produtos = resultado.items;
        this.carregando = false;
      },
      error: (erro) => {
        this.logService.error('Erro ao carregar produtos', 'ProdutosComponent', erro);
        this.carregando = false;
      }
    });
  }

  filtrar(): void {
    this.carregarProdutos();
  }

  // Helper para obter preço principal
  getPreco(produto: Produto): number {
    return produto.precos?.find(p => p.principal)?.valor || 0;
  }
}
