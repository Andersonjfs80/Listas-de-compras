import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  title = 'App Mobile Compras';

  ngOnInit() {
    // Redirecionamento forçado para home conforme pedido do usuário
    // Remove a tela de welcome
    if (window.location.pathname === '/' || window.location.pathname === '/welcome') {
      window.location.href = '/home';
    }
  }
}
