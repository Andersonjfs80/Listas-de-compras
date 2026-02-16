import { Component, signal } from '@angular/core';

@Component({
    selector: 'app-bottom-nav',
    standalone: false,
    templateUrl: './bottom-nav.html',
    styleUrl: './bottom-nav.scss'
})
export class BottomNavComponent {
    protected readonly rotaAtiva = signal('home');

    definirRotaAtiva(rota: string): void {
        this.rotaAtiva.set(rota);
    }

    isAtiva(rota: string): boolean {
        return this.rotaAtiva() === rota;
    }
}
