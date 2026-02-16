import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-welcome',
  template: '',
  styles: [],
  standalone: false
})
export class Welcome implements OnInit {
  ngOnInit() {
    console.log('Welcome Component Inicializado!');
    // Redirecionamento automático removido para exibir mensagem de ajuda
    // O usuário deve clicar no botão para ir ao Gateway (porta 80)
  }
}
