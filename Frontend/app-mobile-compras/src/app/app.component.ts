import { Component, OnInit, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  title = 'App Mobile Compras';
  private router = inject(Router);
  private platformId = inject(PLATFORM_ID);

  ngOnInit() {
    // Redirecionamento forçado para home protegido para SSR
    if (isPlatformBrowser(this.platformId)) {
      if (window.location.pathname === '/' || window.location.pathname === '/welcome') {
        this.router.navigate(['/home']);
      }
    }
  }
}
