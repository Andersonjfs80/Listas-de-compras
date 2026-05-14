import { Component, OnInit } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatListModule } from '@angular/material/list';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ListaComprasService, ListaComprasDto, ItemListaDto } from '../../services/lista-compras.service';
import { AdicionarItemDialogComponent } from './dialogs/adicionar-item-dialog';

interface CategoriaAgrupada {
  nome: string;
  itens: ItemListaDto[];
  destaque?: boolean;
}

@Component({
  selector: 'app-listas',
  templateUrl: './listas.html',
  styleUrls: ['./listas.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatListModule,
    MatCheckboxModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDialogModule,
    DecimalPipe
  ]
})
export class ListasComponent implements OnInit {

  listas: ListaComprasDto[] = [];
  listaSelecionada: ListaComprasDto | null = null;
  categorias: CategoriaAgrupada[] = [];
  carregando = false;
  erro: string | null = null;

  constructor(
    private listaComprasService: ListaComprasService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.carregarListas();
  }

  carregarListas(): void {
    this.carregando = true;
    this.erro = null;

    this.listaComprasService.obterListas().subscribe({
      next: (response) => {
        this.listas = response.listas || [];
        if (this.listas.length > 0) {
          this.selecionarLista(this.listas[0]);
        }
        this.carregando = false;
      },
      error: (err) => {
        console.error('Erro ao carregar listas', err);
        this.erro = 'Não foi possível carregar as listas. Tente novamente.';
        this.carregando = false;
      }
    });
  }

  selecionarLista(lista: ListaComprasDto): void {
    this.listaSelecionada = lista;
    this.agruparPorCategoria(lista.itens);
  }

  private agruparPorCategoria(itens: ItemListaDto[]): void {
    const mapa = new Map<string, ItemListaDto[]>();

    for (const item of itens) {
      const cat = item.categoriaNome || 'Outros';
      if (!mapa.has(cat)) mapa.set(cat, []);
      mapa.get(cat)!.push(item);
    }

    // Itens marcados ficam em grupo especial "No Carrinho"
    const marcados = itens.filter(i => i.marcado);
    const naoMarcados = itens.filter(i => !i.marcado);

    this.categorias = [];

    // Agrupar não marcados por categoria
    const mapaCategoria = new Map<string, ItemListaDto[]>();
    for (const item of naoMarcados) {
      const cat = item.categoriaNome || 'Outros';
      if (!mapaCategoria.has(cat)) mapaCategoria.set(cat, []);
      mapaCategoria.get(cat)!.push(item);
    }

    mapaCategoria.forEach((itensGrupo, nome) => {
      this.categorias.push({ nome, itens: itensGrupo });
    });

    if (marcados.length > 0) {
      this.categorias.push({ nome: 'No Carrinho', itens: marcados, destaque: true });
    }
  }

  toggleItem(item: ItemListaDto): void {
    if (!this.listaSelecionada) return;

    this.listaComprasService.toggleItem(this.listaSelecionada.id, item.id).subscribe({
      next: (response) => {
        item.marcado = response.marcado ?? !item.marcado;
        this.agruparPorCategoria(this.listaSelecionada!.itens);
      },
      error: (err) => {
        console.error('Erro ao marcar item', err);
        this.snackBar.open('Erro ao atualizar item', 'Fechar', { duration: 3000 });
      }
    });
  }

  get totalItens(): number {
    return this.listaSelecionada?.totalItens ?? 0;
  }

  get itensMarcados(): number {
    return this.listaSelecionada?.itensMarcados ?? 0;
  }

  abrirAdicionarItem(): void {
    if (!this.listaSelecionada) return;

    const dialogRef = this.dialog.open(AdicionarItemDialogComponent, {
      width: '90vw',
      maxWidth: '440px',
      data: {
        listaId: this.listaSelecionada.id,
        listaNome: this.listaSelecionada.nome
      }
    });

    dialogRef.afterClosed().subscribe((itemAdicionado: boolean) => {
      if (itemAdicionado) {
        this.snackBar.open('Item adicionado!', 'Ok', { duration: 2500 });
        this.carregarListas();
      }
    });
  }
}
