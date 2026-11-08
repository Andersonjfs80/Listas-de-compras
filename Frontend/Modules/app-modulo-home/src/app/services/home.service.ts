import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { LogService } from '@app/logs';

export interface ProdutoHome {
    id?: string | number;
    nome: string;
    preco: number;
    imagem: string;
    oferta: boolean;
    precoAnterior?: number;
}

// Tipagens estritas das respostas do Backend (C#)
export interface ProdutoPrecoResponse {
    valor: number;
}

export interface ProdutoImagemResponse {
    conteudo: string;
}

export interface ProdutoResponse {
    id: string;
    nome: string;
    preco?: ProdutoPrecoResponse;
    imagem?: ProdutoImagemResponse;
}

export interface FiltrarProdutosResponse {
    produtos?: ProdutoResponse[];
    items?: ProdutoResponse[]; // Fallback caso o backend mude para padrão paginado genérico
}

export interface OfertaResponse {
    id: string;
    produto?: ProdutoResponse;
}

export interface FiltrarOfertasResponse {
    ofertas?: OfertaResponse[];
    items?: OfertaResponse[];
}

@Injectable({
    providedIn: 'root'
})
export class HomeService {

    private apiUrlProduto = environment.apiUrls.produto;
    private apiUrlListaCompras = environment.apiUrls.listaCompras;

    constructor(private http: HttpClient, private logService: LogService) { }

    getProdutosMaisComprados(): Observable<ProdutoHome[]> {
        const payload = { pageNumber: 1, pageSize: 4 };
        return this.http.post<any>(`${this.apiUrlProduto}/produtos/filtrar`, payload).pipe(
            map(resultado => {
                const data = resultado.data || resultado;
                const produtos = data.produtos || data.items || [];
                return produtos.slice(0, 4).map((p: ProdutoResponse) => ({
                    id: p.id,
                    nome: p.nome,
                    preco: p.preco?.valor || 0,
                    imagem: p.imagem?.conteudo || 'https://placehold.co/150x150?text=Produto',
                    oferta: false
                }));
            }),
            catchError((err) => {
                this.logService.error('Erro ao carregar Mais Comprados', 'HomeService', err);
                return of([]);
            })
        );
    }

    getUltimasOfertas(): Observable<ProdutoHome[]> {
        const payload = { pageNumber: 1, pageSize: 4 };
        return this.http.post<any>(`${this.apiUrlListaCompras}/ofertas/filtrar`, payload).pipe(
            map(resultado => {
                const data = resultado.data || resultado;
                const ofertas = data.ofertas || data.items || [];
                return ofertas.slice(0, 4).map((o: OfertaResponse) => ({
                    id: o.produto?.id || o.id,
                    nome: o.produto?.nome || 'Oferta sem nome',
                    preco: o.produto?.preco?.valor || 0,
                    precoAnterior: undefined, // Conforme Regra 14: Se não vem do back, não inventamos.
                    imagem: o.produto?.imagem?.conteudo || 'https://placehold.co/150x150?text=Oferta',
                    oferta: true
                }));
            }),
            catchError((err) => {
                this.logService.error('Erro ao carregar Ofertas', 'HomeService', err);
                return of([]);
            })
        );
    }
}
