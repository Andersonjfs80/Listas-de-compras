import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoadingService {
  private loadingSubject = new BehaviorSubject<boolean>(false);
  
  /**
   * Observable que emite true quando há uma requisição em andamento.
   * Pode ser usado em templates com o pipe async: [disabled]="loadingService.isLoading$ | async"
   */
  isLoading$ = this.loadingSubject.asObservable();

  /**
   * Define o estado de carregamento.
   */
  setLoading(loading: boolean): void {
    this.loadingSubject.next(loading);
  }

  /**
   * Atalho para verificar o estado atual de forma síncrona.
   */
  get isLoading(): boolean {
    return this.loadingSubject.value;
  }
}
