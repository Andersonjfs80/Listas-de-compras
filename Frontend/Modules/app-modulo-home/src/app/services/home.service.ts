import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

export interface ProdutoHome {
    id?: number;
    nome: string;
    preco: number;
    imagem: string;
    oferta: boolean;
    precoAnterior?: number;
}

import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class HomeService {

    // URL do Gateway de Produtos
    private apiUrl = environment.apiUrls.produto;
    private useMockData = true; // Flag MANTIDA como true para usar mocks conforme solicitado

    constructor(private http: HttpClient) { }

    getProdutosMaisComprados(): Observable<ProdutoHome[]> {
        // Se estiver configurado para usar apenas mocks, retorna direto
        if (this.useMockData) {
            console.log('🎭 Usando dados MOCKADOS para Mais Comprados');
            return of(this.getMockMaisComprados());
        }

        // Tentativa de chamada real com fallback para mocks
        return this.http.get<any[]>(`${this.apiUrl}`).pipe(
            map(produtos => produtos.slice(0, 4).map(p => ({
                id: p.id,
                nome: p.nome,
                preco: p.precoBase,
                imagem: p.imagem || 'https://placehold.co/100x100?text=Produto',
                oferta: false
            }))),
            catchError(error => {
                console.warn('⚠️ Backend indisponível, usando dados mockados para Mais Comprados', error);
                return of(this.getMockMaisComprados());
            })
        );
    }

    getUltimasOfertas(): Observable<ProdutoHome[]> {
        // Se estiver configurado para usar apenas mocks, retorna direto
        if (this.useMockData) {
            console.log('🎭 Usando dados MOCKADOS para Ofertas');
            return of(this.getMockOfertas());
        }

        // Tentativa de chamada real com fallback para mocks
        return this.http.get<any[]>(`${this.apiUrl}`).pipe(
            map(produtos => produtos
                .filter(p => p.precoPromocional && p.precoPromocional < p.precoBase)
                .slice(0, 3)
                .map(p => ({
                    id: p.id,
                    nome: p.nome,
                    preco: p.precoPromocional,
                    precoAnterior: p.precoBase,
                    imagem: p.imagem || 'https://placehold.co/100x100?text=Oferta',
                    oferta: true
                }))
            ),
            catchError(error => {
                console.warn('⚠️ Backend indisponível, usando dados mockados para Ofertas', error);
                return of(this.getMockOfertas());
            })
        );
    }

    // Métodos privados para dados mockados
    private getMockMaisComprados(): ProdutoHome[] {
        return [
            {
                id: 1,
                nome: 'Banana Prata',
                preco: 4.99,
                imagem: 'https://placehold.co/150x150/FFFDE7/FBC02D?text=Banana',
                oferta: false
            },
            {
                id: 2,
                nome: 'Leite Integral',
                preco: 5.50,
                imagem: 'https://placehold.co/150x150/E3F2FD/1976D2?text=Leite',
                oferta: false
            },
            {
                id: 3,
                nome: 'Pão Francês',
                preco: 12.90,
                imagem: 'https://placehold.co/150x150/FFF3E0/E64A19?text=Pão',
                oferta: false
            },
            {
                id: 4,
                nome: 'Ovos Dúzia',
                preco: 14.00,
                imagem: 'https://placehold.co/150x150/FAFAFA/FBC02D?text=Ovos',
                oferta: false
            }
        ];
    }

    private getMockOfertas(): ProdutoHome[] {
        return [
            {
                id: 5,
                nome: 'Café Torrado',
                preco: 18.90,
                precoAnterior: 22.00,
                imagem: 'https://placehold.co/150x150/EFEBE9/3E2723?text=Café',
                oferta: true
            },
            {
                id: 6,
                nome: 'Azeite Oliva',
                preco: 35.00,
                precoAnterior: 42.00,
                imagem: 'https://placehold.co/150x150/F1F8E9/33691E?text=Azeite',
                oferta: true
            },
            {
                id: 7,
                nome: 'Sabão em Pó',
                preco: 28.50,
                precoAnterior: 32.90,
                imagem: 'https://placehold.co/150x150/E8EAF6/1A237E?text=Sabão',
                oferta: true
            },
            {
                id: 8,
                nome: 'Refrigerante',
                preco: 8.00,
                precoAnterior: 10.00,
                imagem: 'https://placehold.co/150x150/FFEBEE/B71C1C?text=Refri',
                oferta: true
            }
        ];
    }

    /**
     * Método para alternar entre modo mock e modo real
     * Para uso futuro quando quiser integrar com backend real
     */
    setUseMockData(useMock: boolean): void {
        this.useMockData = useMock;
        console.log(`🔧 Modo de dados alterado para: ${useMock ? 'MOCK' : 'REAL'}`);
    }
}
