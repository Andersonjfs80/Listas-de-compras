import { Injectable, inject } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LOG_CONFIG, LogConfig } from '../log.models';
import { LogService } from '../log.service';
import { StorageService } from '../services/storage.service';

@Injectable()
export class ModuleHeaderInterceptor implements HttpInterceptor {
    private config = inject<LogConfig>(LOG_CONFIG);
    private logService = inject(LogService);
    private storageService = inject(StorageService);

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

        let sessionId = this.storageService.getItem('SESSAO-ID');
        if (!sessionId) {
            sessionId = generateUUID();
            this.storageService.setItem('SESSAO-ID', sessionId);
        }
        finalSessionId = sessionId;

        // 2. Message ID do Módulo
        const storageKeyModulo = `MESSAGE-ID-${appModulo}`;
        messageIdModulo = this.storageService.getItem(storageKeyModulo) || generateUUID();
        if (!this.storageService.getItem(storageKeyModulo)) {
            this.storageService.setItem(storageKeyModulo, messageIdModulo);
        }

        // 3. Hardware ID
        let hardwareId = this.storageService.getItem('HARDWARE-ID');
        const isProdValidPattern = hardwareId && (hardwareId.startsWith('PC-') || hardwareId.startsWith('MOB-'));

        if (!isProdValidPattern) {
            const userAgent = typeof navigator !== 'undefined' ? navigator.userAgent : '';
            const isMobile = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini|Mobile|mobile|CriOS/i.test(userAgent);
            const prefixo = isMobile ? 'MOB' : 'PC';

            hardwareId = `${prefixo}-${generateUUID()}`;
            this.storageService.setItem('HARDWARE-ID', hardwareId);
        }
        finalHardwareId = hardwareId || 'PC-UNKNOWN';

        let usuarioId = '';
        const storedUsuarioId = this.storageService.getItem('USUARIO-ID');
        if (storedUsuarioId) {
            usuarioId = storedUsuarioId;
        } else {
            const userJson = this.storageService.getItem('user');
            if (userJson) {
                try {
                    const user = JSON.parse(userJson);
                    usuarioId = user.id || user.Id || '';
                    if (usuarioId) {
                        this.storageService.setItem('USUARIO-ID', usuarioId);
                    }
                } catch (e) {
                    // ignora erro no parse do JSON
                }
            }
        }

        const traceId = generateTraceId();
        const headers: { [key: string]: string } = {
            'sigla-aplicacao': String(appModulo),
            'message-id': generateUUID(),
            'message-id-modulo': String(messageIdModulo),
            'sessao-id': finalSessionId,
            'hardware-id': finalHardwareId,
            'x-trace-id': traceId,
            'usuario-id': usuarioId
        };

        // Obter token de forma robusta e defensiva com verificação de EXPIRAÇÃO (JWT)
        let token: string | null = null;
        const rawToken = this.storageService.getItem('token');
        if (rawToken && rawToken !== 'null' && rawToken !== 'undefined' && rawToken.trim() !== '') {
            try {
                // O JWT tem 3 partes separadas por ponto: header, payload, signature
                const payloadBase64 = rawToken.split('.')[1];
                if (payloadBase64) {
                    const decodedPayload = JSON.parse(atob(payloadBase64));
                    // Verifica se expirou (multiplica por 1000 pois exp é em segundos)
                    const isExpired = (decodedPayload.exp * 1000) < Date.now();
                    
                    if (isExpired) {
                        this.logService.warn('[AUTH] Token expirado interceptado no Frontend. Removendo credenciais locais.', 'ModuleHeaderInterceptor');
                        this.storageService.removeItem('token');
                        this.storageService.removeItem('SESSAO-ID');
                        this.storageService.removeItem('USUARIO-ID');
                        // token continua null para disparar o log de Unauthorized logo abaixo
                    } else {
                        token = rawToken; // Token válido
                    }
                } else {
                    token = rawToken; // Fallback caso não seja formato JWT padrão
                }
            } catch (e) {
                token = rawToken; // Se falhar o parse (não for JWT), repassa pra frente (o backend vai recusar)
            }
        }

        if (token) {
            headers['authorization'] = `Bearer ${token}`;
        }

        // Validação estrita do token para rotas protegidas
        const url = request.url.toLowerCase();
        const isPublicEndpoint = url.includes('/logs') || 
                                 url.includes('/login') || 
                                 url.includes('/cadastrar') || 
                                 url.includes('/resetar-senha') || 
                                 url.includes('/cadastrar-senha') ||
                                 url.includes('/health');

        if (!isPublicEndpoint && !token) {
            // Log de erro mandatório para o Kibana
            this.logService.error(
                `Tentativa de acesso a recurso protegido sem token de autorização. Recurso: ${request.method} ${request.url}`,
                'ModuleHeaderInterceptor',
                {
                    tipo: 'error',
                    method: request.method,
                    fullUrl: request.url,
                    traceId: traceId,
                    statusCode: 401,
                    statusCodeProcessamento: 'UNAUTHORIZED',
                    message: 'Cabeçalho de autorização ausente em rota protegida'
                }
            );
        }

        const modifiedRequest = request.clone({
            setHeaders: headers
        });

        return next.handle(modifiedRequest);
    }
}
