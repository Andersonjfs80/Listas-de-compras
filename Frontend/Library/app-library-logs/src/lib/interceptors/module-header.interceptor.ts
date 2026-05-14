import { Injectable, inject } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LOG_CONFIG, LogConfig } from '../log.models';

@Injectable()
export class ModuleHeaderInterceptor implements HttpInterceptor {
    private config = inject<LogConfig>(LOG_CONFIG);

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const appModulo = this.config.appName || 'APP-INTERCEPTOR';

        // Função helper para garantir UUID mesmo sem suporte a crypto.randomUUID
        const generateUUID = () => {
            if (typeof crypto !== 'undefined' && crypto.randomUUID) {
                try {
                    return crypto.randomUUID();
                } catch (e) { /* fallback */ }
            }
            return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
                const r = (Math.random() * 16) | 0;
                const v = c === 'x' ? r : (r & 0x3) | 0x8;
                return v.toString(16);
            });
        };

        const generateTraceId = () => {
            return Math.random().toString(16).substring(2, 18);
        };

        let finalSessionId: string = generateUUID();
        let messageIdModulo: string = generateUUID();
        let finalHardwareId: string = 'PC-UNKNOWN';

        if (typeof localStorage !== 'undefined') {
            // 1. Session ID
            let sessionId = localStorage.getItem('SESSAO-ID');
            if (!sessionId) {
                sessionId = generateUUID();
                localStorage.setItem('SESSAO-ID', sessionId);
            }
            finalSessionId = sessionId;

            // 2. Message ID do Módulo
            const storageKeyModulo = `MESSAGE-ID-${appModulo}`;
            messageIdModulo = localStorage.getItem(storageKeyModulo) || generateUUID();
            if (!localStorage.getItem(storageKeyModulo)) {
                localStorage.setItem(storageKeyModulo, messageIdModulo);
            }

            // 3. Hardware ID
            let hardwareId = localStorage.getItem('HARDWARE-ID');
            const isProdValidPattern = hardwareId && (hardwareId.startsWith('PC-') || hardwareId.startsWith('MOB-'));

            if (!isProdValidPattern) {
                const userAgent = typeof navigator !== 'undefined' ? navigator.userAgent : '';
                const isMobile = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini|Mobile|mobile|CriOS/i.test(userAgent);
                const prefixo = isMobile ? 'MOB' : 'PC';

                hardwareId = `${prefixo}-${generateUUID()}`;
                localStorage.setItem('HARDWARE-ID', hardwareId);
            }
            finalHardwareId = hardwareId || 'PC-UNKNOWN';
        }

        const modifiedRequest = request.clone({
            setHeaders: {
                'SIGLA-APLICACAO': String(appModulo),
                'MESSAGE-ID': generateUUID(),
                'MESSAGE-ID-MODULO': String(messageIdModulo),
                'SESSAO-ID': finalSessionId,
                'HARDWARE-ID': finalHardwareId,
                'X-Trace-Id': generateTraceId(),
                'USUARIO-ID': typeof localStorage !== 'undefined' ? localStorage.getItem('USUARIO-ID') || '' : ''
            }
        });

        return next.handle(modifiedRequest);
    }
}
