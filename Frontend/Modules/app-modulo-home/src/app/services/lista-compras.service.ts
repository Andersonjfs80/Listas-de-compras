import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

// ---- DTOs ----

export interface ListaComprasDto {
    id: string;
    nome: string;
    dataCriacao: string;
    totalItens: number;
    itensMarcados: number;
    itens: ItemListaDto[];
}

export interface ItemListaDto {
    id: string;
    nomeProduto: string;
    quantidade: number;
    unidadeMedida: string;
    marcado: boolean;
    categoriaNome?: string;
    imagem?: string;
}

export interface ListaComprasResponse {
    listas: ListaComprasDto[];
    mensagem?: string;
}

export interface CriarListaRequest {
    nome: string;
}

export interface AdicionarItemRequest {
    nomeProduto: string;
    quantidade: number;
    unidadeMedida: string;
    categoriaNome?: string;
    imagem?: string;
}

// ---- Service ----

@Injectable({
    providedIn: 'root'
})
export class ListaComprasService {

    private apiUrl = environment.apiUrls.listaCompras;

    constructor(private http: HttpClient) { }

    private getUsuarioId(): string {
        if (typeof localStorage === 'undefined') return '';
        // Extrai o usuarioId do token JWT armazenado no localStorage
        const token = localStorage.getItem('token');
        if (!token) return '';
        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            return payload.sub || payload.userId || payload.nameid || '';
        } catch {
            return '';
        }
    }

    private headers(): HttpHeaders {
        return new HttpHeaders({
            'USUARIO-ID': this.getUsuarioId()
        });
    }

    obterListas(): Observable<ListaComprasResponse> {
        return this.http.get<ListaComprasResponse>(
            `${this.apiUrl}/listas`,
            { headers: this.headers() }
        );
    }

    criarLista(request: CriarListaRequest): Observable<any> {
        return this.http.post<any>(
            `${this.apiUrl}/listas`,
            request,
            { headers: this.headers() }
        );
    }

    adicionarItem(listaId: string, request: AdicionarItemRequest): Observable<any> {
        return this.http.post<any>(
            `${this.apiUrl}/listas/${listaId}/itens`,
            request,
            { headers: this.headers() }
        );
    }

    toggleItem(listaId: string, itemId: string): Observable<any> {
        return this.http.put<any>(
            `${this.apiUrl}/listas/${listaId}/itens/${itemId}/toggle`,
            {},
            { headers: this.headers() }
        );
    }

    removerItem(listaId: string, itemId: string): Observable<any> {
        return this.http.delete<any>(
            `${this.apiUrl}/listas/${listaId}/itens/${itemId}`,
            { headers: this.headers() }
        );
    }
}
