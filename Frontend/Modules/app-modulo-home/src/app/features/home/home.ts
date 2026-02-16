import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { HomeService, ProdutoHome } from '../../services/home.service';

interface Categoria {
  nome: string;
  icone: string;
  cor: string;
  selecionada?: boolean;
}

interface Banner {
  titulo: string;
  subtitulo: string;
  textoBotao: string;
  corFundo: string;
  imagem?: string;
}

@Component({
  selector: 'app-home',
  templateUrl: './home.html',
  styleUrls: ['./home.scss'],
  standalone: true,
  imports: [
    CommonModule,
    HttpClientModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule
  ]
})
export class HomeComponent implements OnInit {

  saudacao = 'Bem-vindo';
  nomeUsuario = 'Visitante'; // Futuro: pegar do AuthService

  categorias: Categoria[] = [
    { nome: 'Frutas', icone: 'nutrition', cor: '#E8F5E9', selecionada: true }, // mat-icon: nutrition (ou outro similar)
    { nome: 'Legumes', icone: 'eco', cor: '#F1F8E9' },
    { nome: 'Padaria', icone: 'cake', cor: '#FFF3E0' },
    { nome: 'Bebidas', icone: 'local_drink', cor: '#E3F2FD' },
    { nome: 'Carnes', icone: 'restaurant', cor: '#FFEBEE' },
    { nome: 'Peixes', icone: 'set_meal', cor: '#E0F7FA' }
  ];

  banners: Banner[] = [
    {
      titulo: 'Ofertas de Verão',
      subtitulo: 'Desconto de 25%',
      textoBotao: 'Aproveitar',
      corFundo: 'linear-gradient(135deg, #4CAF50 0%, #8BC34A 100%)',
      imagem: 'assets/images/banner-fruits.svg'
    },
    {
      titulo: 'Padaria Fresquinha',
      subtitulo: 'Pães e Bolos com 15% OFF',
      textoBotao: 'Ver Produtos',
      corFundo: 'linear-gradient(135deg, #FF9800 0%, #FFB74D 100%)',
      imagem: 'assets/images/banner-bakery.svg'
    },
    {
      titulo: 'Carnes Importadas',
      subtitulo: 'Cortes Especiais',
      textoBotao: 'Comprar Agora',
      corFundo: 'linear-gradient(135deg, #D32F2F 0%, #E57373 100%)',
      imagem: 'assets/images/banner-meat.svg'
    }
  ];

  bannerAtivoIndex = 0;
  private imageRetryCount = new Map<string, number>();

  // Arrays de produtos (serão preenchidos pelo serviço)
  maisComprados: ProdutoHome[] = [];
  ultimasOfertas: ProdutoHome[] = [];

  loading = true;

  constructor(private homeService: HomeService) { }

  ngOnInit() {
    this.definirSaudacao();
    this.iniciarCarrossel();
    this.carregarDados();
  }

  ngOnDestroy() {
    if (this.intervalId) clearInterval(this.intervalId);
  }

  private intervalId: any;

  private iniciarCarrossel() {
    this.intervalId = setInterval(() => {
      this.bannerAtivoIndex = (this.bannerAtivoIndex + 1) % this.banners.length;
    }, 5000); // 5 segundos com transição suave CSS
  }

  setBanner(index: number) {
    this.bannerAtivoIndex = index;
    // Reinicia timer ao interagir
    clearInterval(this.intervalId);
    this.iniciarCarrossel();
  }

  private definirSaudacao() {
    const hora = new Date().getHours();
    if (hora < 12) this.saudacao = 'Bom dia';
    else if (hora < 18) this.saudacao = 'Boa tarde';
    else this.saudacao = 'Boa noite';
  }

  private carregarDados() {
    this.loading = true;

    // Carregar produtos mais comprados
    this.homeService.getProdutosMaisComprados().subscribe({
      next: (data) => {
        this.maisComprados = data;
      },
      error: (error) => {
        console.error('Erro ao carregar mais comprados:', error);
        this.loading = false;
      }
    });

    // Carregar últimas ofertas
    this.homeService.getUltimasOfertas().subscribe({
      next: (data) => {
        this.ultimasOfertas = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar ofertas:', error);
        this.loading = false;
      }
    });
  }

  selecionarCategoria(categoria: Categoria) {
    this.categorias.forEach(c => c.selecionada = false);
    categoria.selecionada = true;
    // Logica de filtro viria aqui
  }

  onImageError(event: Event, imageUrl: string) {
    const img = event.target as HTMLImageElement;
    const currentRetries = this.imageRetryCount.get(imageUrl) || 0;

    if (currentRetries < 3) {
      // Incrementa tentativas
      this.imageRetryCount.set(imageUrl, currentRetries + 1);
      // Tenta recarregar a imagem
      img.src = imageUrl + '?retry=' + currentRetries;
    } else {
      // Após 3 tentativas, remove o onerror para evitar loop
      img.onerror = null;
      console.error(`Falha ao carregar imagem após 3 tentativas: ${imageUrl}`);
    }
  }
}
