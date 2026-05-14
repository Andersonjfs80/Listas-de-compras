import { Injectable, inject } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpResponse } from '@angular/common/http';
import { from, Observable, of } from 'rxjs';
import { switchMap, map, catchError, finalize } from 'rxjs/operators';
import { LOG_CONFIG } from '../log.models';
import type { LogConfig } from '../log.models';
import { CryptoService } from '../crypto.service';
import { LoadingService } from '../services/loading.service';

@Injectable()
export class EncryptionInterceptor implements HttpInterceptor {
  private config = inject<LogConfig>(LOG_CONFIG);
  private crypto = inject(CryptoService);
  private loading = inject(LoadingService);

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // 1. Elegibilidade
    const isEligible = this.config.enableBodyEncryption && request.body && 
                       (request.method === 'POST' || request.method === 'PUT' || request.method === 'PATCH');

    if (!isEligible) {
      return next.handle(request).pipe(
        switchMap(event => this.handleResponse(event))
      );
    }

    // 2. ATIVA O LOADING (Anti-duplo clique)
    this.loading.setLoading(true);

    // 3. Criptografia Discreta (Envelope JSON + Header Ofuscado)
    return from(this.crypto.encryptBodyJose(request.body)).pipe(
      switchMap(encryptedToken => {
        const encryptedRequest = request.clone({
          body: { data: encryptedToken },
          headers: request.headers.set('Content-Type', 'application/json')
                                    .set('X-Sec-Key', '1')
        });
        
        return next.handle(encryptedRequest).pipe(
          switchMap(event => this.handleResponse(event))
        );
      }),
      // 4. DESATIVA O LOADING (Garante que sempre rode, como um try-finally)
      finalize(() => this.loading.setLoading(false))
    );
  }

  /**
   * Tenta descriptografar a resposta caso contenha o header discreto.
   */
  private handleResponse(event: HttpEvent<any>): Observable<HttpEvent<any>> {
    if (event instanceof HttpResponse && event.headers.get('X-Sec-Key') === '1') {
      const jwe = event.body?.data || event.body;
      return from(this.crypto.decryptBodyJose(jwe)).pipe(
        map(decryptedBody => {
          return event.clone({ body: decryptedBody });
        }),
        catchError(err => {
          console.error('[EncryptionInterceptor] Erro interno de processamento:', err);
          return of(event);
        })
      );
    }
    return of(event);
  }
}
