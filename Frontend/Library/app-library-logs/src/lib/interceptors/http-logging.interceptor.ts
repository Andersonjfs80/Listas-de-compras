import { Injectable, inject, Inject } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { LogService } from '../log.service';
import { LOG_CONFIG } from '../log.models';
import type { LogConfig } from '../log.models';

@Injectable()
export class HttpLoggingInterceptor implements HttpInterceptor {
  private logService = inject(LogService);
  private config = inject<LogConfig>(LOG_CONFIG);

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // EVITAR LOOP INFINITO
    const logUrl = this.config.apiUrl;
    if (logUrl && request.url.includes(logUrl)) {
      return next.handle(request);
    }

    const startTime = Date.now();
    const headers: any = {};
    request.headers.keys().forEach(key => {
        headers[key] = request.headers.get(key);
    });

    const traceId = request.headers.get('X-Trace-Id') || undefined;
    const safePath = request.url.startsWith('http') ? new URL(request.url).pathname : request.url.split('?')[0];

    // 1. LOG DE REQUEST (ENTRADA) - Imediato antes de disparar
    this.logService.log('Info', `Iniciando Chamada: ${request.method} ${request.url}`, 'HttpLoggingInterceptor', {
      tipo: 'request',
      traceId: traceId,
      method: request.method,
      fullUrl: request.url,
      path: safePath,
      headers: headers,
      body: request.body
    });
    
    return next.handle(request).pipe(
      tap({
        next: (event) => {
          if (event instanceof HttpResponse) {
            const duration = Date.now() - startTime;
            
            const responseHeaders: any = {};
            event.headers.keys().forEach(key => {
              responseHeaders[key] = event.headers.get(key);
            });

            // 2. LOG DE RESPONSE (SAÍDA - SUCESSO)
            this.logService.log('Info', `Chamada Finalizada: ${event.status} ${request.method} ${request.url}`, 'HttpLoggingInterceptor', {
              tipo: 'response',
              traceId: traceId,
              method: request.method,
              fullUrl: request.url,
              path: safePath,
              statusCode: event.status,
              durationMs: duration,
              headers: headers,
              responseHeaders: responseHeaders,
              body: request.body,
              response: event.body
            });
          }
        },
        error: (error) => {
          const duration = Date.now() - startTime;
          // 3. LOG DE RESPONSE (SAÍDA - ERRO)
          this.logService.error(`Erro na chamada: ${error.status} ${request.method} ${request.url}`, 'HttpLoggingInterceptor', {
            tipo: 'error',
            traceId: traceId,
            method: request.method,
            fullUrl: request.url,
            path: safePath,
            statusCode: error.status || 0,
            durationMs: duration,
            headers: headers,
            body: request.body,
            error: error
          });
        }
      })
    );
  }
}
