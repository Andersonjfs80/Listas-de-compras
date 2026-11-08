import { Injectable, inject } from '@angular/core';
import {
  HttpEvent,
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpResponse,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, from, of, throwError, defer } from 'rxjs';
import { map, catchError, switchMap, finalize } from 'rxjs/operators';
import { CryptoService } from '../crypto.service';
import { LOG_CONFIG } from '../log.models';
import type { LogConfig } from '../log.models';
import { LoadingService } from '../services/loading.service';
import { LogService } from '../log.service';

@Injectable()
export class EncryptionInterceptor implements HttpInterceptor {
  private crypto = inject(CryptoService);
  private environment = inject<LogConfig>(LOG_CONFIG);
  private loading = inject(LoadingService);
  private logService = inject(LogService);

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const isNonProd = this.environment.environment === 'DEV' ||
      this.environment.environment === 'HML' ||
      this.environment.environment === 'Development' ||
      this.environment.environment === 'Homologation';

    this.logService.info(`[EncryptionInterceptor] Verificando ambiente: ${this.environment.environment}`, 'EncryptionInterceptor');

    const isEncryptionEnabled = isNonProd ? this.environment.enableBodyEncryption : true;

    this.loading.setLoading(true);

    if (!isEncryptionEnabled) {
      return next.handle(req).pipe(finalize(() => this.loading.setLoading(false)));
    }

    return defer(() => this.crypto.encryptBodyJose(req.body)).pipe(
      switchMap(encryptedBody => {
        let encryptedRequest = req;
        if (req.method === 'POST' || req.method === 'PUT' || req.method === 'PATCH') {
          encryptedRequest = req.clone({ body: { data: encryptedBody } });
        }

        return next.handle(encryptedRequest).pipe(
          switchMap(event => this.handleResponse(event)),
          catchError((error: HttpErrorResponse) => throwError(() => error))
        );
      }),
      finalize(() => this.loading.setLoading(false))
    );
  }

  private handleResponse(event: HttpEvent<any>): Observable<HttpEvent<any>> {
    if (event instanceof HttpResponse) {
      this.logService.info('[EncryptionInterceptor] Analisando corpo de resposta recebido.', 'EncryptionInterceptor');

      const jwe = event.body?.data || event.body;

      if (typeof jwe === 'string' && (jwe.match(/\./g) || []).length >= 4) {
        this.logService.info('[EncryptionInterceptor] Envelope JWE detectado. Iniciando descriptografia...', 'EncryptionInterceptor');

        return defer(() => this.crypto.decryptBodyJose(jwe)).pipe(
          map(decryptedBody => {
            this.logService.info('[EncryptionInterceptor] Corpo de resposta descriptografado com sucesso.', 'EncryptionInterceptor');
            return event.clone({ body: decryptedBody });
          }),
          catchError(err => {
            this.logService.error('[EncryptionInterceptor] Falha ao descriptografar corpo de resposta (JWE inválido ou chave incorreta).', 'EncryptionInterceptor', err);
            return of(event);
          })
        );
      }
    }
    return of(event);
  }
}
