import { Injectable, Inject } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LOG_CONFIG } from '../log.models';
import type { LogConfig } from '../log.models';

@Injectable()
export class ModuleHeaderInterceptor implements HttpInterceptor {
    constructor(@Inject(LOG_CONFIG) private config: LogConfig) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const appModulo = this.config.appName || 'APP-INTERCEPTOR';

        // 1. Session ID (Persistente até que o usuário faça logout/login)
        let sessionId = localStorage.getItem('SESSAO-ID');
        if (!sessionId) {
            sessionId = crypto.randomUUID();
            localStorage.setItem('SESSAO-ID', sessionId);
        }
        const finalSessionId: string = sessionId || crypto.randomUUID();

        // 2. Message ID do Módulo (Persistente e Único por Módulo - Regra do Usuário)
        const storageKeyModulo = `MESSAGE-ID-${appModulo}`;
        let messageIdModulo = localStorage.getItem(storageKeyModulo) || crypto.randomUUID();
        if (!localStorage.getItem(storageKeyModulo)) {
            localStorage.setItem(storageKeyModulo, messageIdModulo);
        }

        // 3. Hardware ID (Identificação de Dispositivo - Regra: PC ou MOB)
        let hardwareId = localStorage.getItem('HARDWARE-ID');
        const isProdValidPattern = hardwareId && (hardwareId.startsWith('PC-') || hardwareId.startsWith('MOB-'));

        if (!isProdValidPattern) {
            const userAgent = navigator.userAgent || '';
            const isMobile = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini|Mobile|mobile|CriOS/i.test(userAgent);
            const prefixo = isMobile ? 'MOB' : 'PC';

            // Fallback para ambientes sem crypto.randomUUID (como HTTP sem SSL)
            const uuid = (typeof crypto !== 'undefined' && crypto.randomUUID)
                ? crypto.randomUUID()
                : Math.random().toString(36).substring(2, 15) + Math.random().toString(36).substring(2, 15);

            hardwareId = `${prefixo}-${uuid}`;
            localStorage.setItem('HARDWARE-ID', hardwareId);
        }

        // 4. Clone da requisição com os headers mandatórios sincronizados com o Backend
        const finalHardwareId: string = hardwareId || 'PC-UNKNOWN';

        const modifiedRequest = request.clone({
            setHeaders: {
                'SIGLA-APLICACAO': String(appModulo),
                'MESSAGE-ID': crypto.randomUUID(),
                'MESSAGE-ID-MODULO': String(messageIdModulo),
                'SESSAO-ID': finalSessionId,
                'HARDWARE-ID': finalHardwareId
            }
        });

        return next.handle(modifiedRequest);
    }
}
