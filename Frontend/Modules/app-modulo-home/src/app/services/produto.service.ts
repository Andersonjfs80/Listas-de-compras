import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

export interface Produto {
    id: string;
    nome: string;
    nomeCurto: string;
    ativo: boolean;
    categoriaId: string;
    categoria: {
        id: string;
        nome: string;
    };
    precos: {
        valor: number;
        unidade: string;
        principal: boolean;
    }[];
    imagens: {
        url: string;
        principal: boolean;
    }[];
}

export interface UnidadeMedida {
    id: string;
    sigla: string;
    descricao: string;
    fatorConversao: number;
    ativo: boolean;
}

export interface TipoEstabelecimento {
    id: string;
    nome: string;
    ativo: boolean;
}

export interface PagedResult<T> {
    items: T[];
    totalCount: number;
}

import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class ProdutoService {
    private apiUrl = environment.apiUrls.produto; // Centralizado no environment
    // Via Nginx -> Produto Gateway

    constructor(private http: HttpClient) { }

    listarProdutos(
        page: number = 1,
        pageSize: number = 20,
        tipoEstabelecimentoId?: string
    ): Observable<PagedResult<Produto>> {
        const payload: any = {
            pageNumber: page,
            pageSize: pageSize
        };

        if (tipoEstabelecimentoId) {
            payload.tipoEstabelecimentoId = tipoEstabelecimentoId;
        }

        return this.http.post<PagedResult<Produto>>(`${this.apiUrl}/produtos/filtrar`, payload);
    }

    listarTiposEstabelecimento(): Observable<TipoEstabelecimento[]> {
        return this.http.get<TipoEstabelecimento[]>(`${this.apiUrl}/tipos-estabelecimento`);
    }

    listarUnidades(): Observable<UnidadeMedida[]> {
        return this.http.get<any>(`${this.apiUrl}/unidades-medida`).pipe(
            map((resultado: any) => resultado.unidades ?? resultado ?? [])
        );
    }
}
