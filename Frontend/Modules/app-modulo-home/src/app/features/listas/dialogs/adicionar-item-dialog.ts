import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ListaComprasService, AdicionarItemRequest } from '../../../services/lista-compras.service';
import { ProdutoService, UnidadeMedida } from '../../../services/produto.service';

export interface AdicionarItemDialogData {
    listaId: string;
    listaNome: string;
}

@Component({
    selector: 'app-adicionar-item-dialog',
    templateUrl: './adicionar-item-dialog.html',
    styleUrls: ['./adicionar-item-dialog.scss'],
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        MatDialogModule,
        MatFormFieldModule,
        MatInputModule,
        MatSelectModule,
        MatButtonModule,
        MatIconModule,
        MatProgressSpinnerModule
    ]
})
export class AdicionarItemDialogComponent implements OnInit {

    nomeProduto = '';
    quantidade = 1;
    unidadeMedida = '';
    categoriaNome = '';
    salvando = false;
    carregandoUnidades = true;
    erro: string | null = null;

    unidades: UnidadeMedida[] = [];

    constructor(
        private dialogRef: MatDialogRef<AdicionarItemDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: AdicionarItemDialogData,
        private listaComprasService: ListaComprasService,
        private produtoService: ProdutoService
    ) { }

    ngOnInit(): void {
        this.produtoService.listarUnidades().subscribe({
            next: (dados) => {
                this.unidades = dados;
                // Selecionar 'UN' como padrão se existir, senão o primeiro
                const padrao = dados.find(u => u.sigla.toUpperCase() === 'UN') ?? dados[0];
                this.unidadeMedida = padrao?.sigla ?? '';
                this.carregandoUnidades = false;
            },
            error: () => {
                // Fallback para lista mínima caso API falhe
                this.unidades = [
                    { id: '', sigla: 'UN', descricao: 'Unidade', fatorConversao: 1, ativo: true },
                    { id: '', sigla: 'KG', descricao: 'Quilograma', fatorConversao: 1, ativo: true },
                    { id: '', sigla: 'L', descricao: 'Litro', fatorConversao: 1, ativo: true },
                ];
                this.unidadeMedida = 'UN';
                this.carregandoUnidades = false;
            }
        });
    }

    get formularioValido(): boolean {
        return this.nomeProduto.trim().length >= 2 && this.quantidade > 0 && !!this.unidadeMedida;
    }

    cancelar(): void {
        this.dialogRef.close(false);
    }

    adicionar(): void {
        if (!this.formularioValido) return;

        this.salvando = true;
        this.erro = null;

        const request: AdicionarItemRequest = {
            nomeProduto: this.nomeProduto.trim(),
            quantidade: this.quantidade,
            unidadeMedida: this.unidadeMedida,
            categoriaNome: this.categoriaNome.trim() || undefined
        };

        this.listaComprasService.adicionarItem(this.data.listaId, request).subscribe({
            next: () => {
                this.dialogRef.close(true);
            },
            error: (err) => {
                console.error('Erro ao adicionar item', err);
                this.erro = 'Não foi possível adicionar o item. Verifique os dados e tente novamente.';
                this.salvando = false;
            }
        });
    }
}
