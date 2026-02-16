import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-toolbar',
  standalone: false,
  templateUrl: './toolbar.html',
  styleUrl: './toolbar.scss',
})
export class Toolbar {
  @Input() titulo = 'Minha Lista';
  @Input() mostrarMenuIcon = true;
  @Input() mostrarAcoes = true;

  @Output() menuClick = new EventEmitter<void>();
  @Output() adicionarClick = new EventEmitter<void>();
  @Output() maisOpcoesClick = new EventEmitter<void>();

  onMenuClick(): void {
    this.menuClick.emit();
  }

  onAdicionarClick(): void {
    this.adicionarClick.emit();
  }

  onMaisOpcoesClick(): void {
    this.maisOpcoesClick.emit();
  }
}
