import { Component, signal } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.scss'
})
export class AppComponent {
  protected readonly title = signal('app-modulo-autenticacao');
}
