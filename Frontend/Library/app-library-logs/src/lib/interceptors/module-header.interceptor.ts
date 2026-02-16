import { Injectable, Inject } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LOG_CONFIG } from '../log.models';
import type { LogConfig } from '../log.models';

@Injectable()
export class ModuleHeaderInterceptor implements HttpInterceptor {
    constructor(@Inject(LOG_CONFIG) private config: LogConfig) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const appModulo = this.config.appName || 'APP-DESCONHECIDO';

        // 1. Session ID (Persistente durante o tempo de vida da aba/janela)
        let sessionId = sessionStorage.getItem('SESSAO-ID');
        if (!sessionId) {
            sessionId = crypto.randomUUID();
            sessionStorage.setItem('SESSAO-ID', sessionId);
        }

        // 2. Hardware ID (Persistente no dispositivo)
        let hardwareId = localStorage.getItem('HARDWARE-ID');
        if (!hardwareId) {
            hardwareId = crypto.randomUUID();
            localStorage.setItem('HARDWARE-ID', hardwareId);
        }

        // 3. Clone da requisição com os headers mandatórios especializados
        const modifiedRequest = request.clone({
            setHeaders: {
                'SIGLA-APLICACAO-MODULO': appModulo,
                'MESSAGE-ID-MODULO': crypto.randomUUID(),
                'SESSAO-ID': sessionId,
                'HARDWARE-ID': hardwareId
            }
        });

        return next.handle(modifiedRequest);
    }
}
