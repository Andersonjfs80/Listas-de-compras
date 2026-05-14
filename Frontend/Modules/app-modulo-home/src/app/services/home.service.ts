import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';

export interface ProdutoHome {
    id?: number;
    nome: string;
    preco: number;
    imagem: string;
    oferta: boolean;
    precoAnterior?: number;
}

@Injectable({
    providedIn: 'root'
})
export class HomeService {

    private apiUrlProduto = environment.apiUrls.produto;
    private apiUrlListaCompras = environment.apiUrls.listaCompras;

    constructor(private http: HttpClient) { }

    getProdutosMaisComprados(): Observable<ProdutoHome[]> {
        return this.http.get<any>(`${this.apiUrlProduto}/produtos?pageNumber=1&pageSize=4`).pipe(
            map(resultado => (resultado.items || resultado || []).slice(0, 4).map((p: any) => ({
                id: p.id,
                nome: p.nome,
                preco: p.precos?.find((pr: any) => pr.principal)?.valor || p.precoBase || 0,
                imagem: p.imagens?.find((img: any) => img.principal)?.url
                    || p.imagem
                    || 'https://placehold.co/150x150?text=Produto',
                oferta: false
            }))),
            catchError(() => {
                console.warn('⚠️ Backend de produtos indisponível');
                return [];
            })
        );
    }

    getUltimasOfertas(): Observable<ProdutoHome[]> {
        return this.http.get<any>(`${this.apiUrlListaCompras}/ofertas?pageNumber=1&pageSize=4`).pipe(
            map(resultado => {
                const ofertas = resultado.ofertas || resultado.items || resultado || [];
                return ofertas.slice(0, 4).map((o: any) => ({
                    id: o.id,
                    nome: o.nomeProduto,
                    preco: o.precoAtual,
                    precoAnterior: o.precoAnterior,
                    imagem: o.imagem || 'https://placehold.co/150x150?text=Oferta',
                    oferta: true
                }));
            }),
            catchError(() => {
                console.warn('⚠️ Backend de ofertas indisponível');
                return [];
            })
        );
    }
}
