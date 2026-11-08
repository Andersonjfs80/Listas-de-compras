import { Injectable, Inject, inject, Injector } from '@angular/core';
import { HttpClient, HttpBackend } from '@angular/common/http';
import { SystemLog, LogLevel, LOG_CONFIG } from './log.models';
import type { LogConfig } from './log.models';
import { catchError } from 'rxjs/operators';
import { of } from 'rxjs';
import { StorageService } from './services/storage.service';

@Injectable({
    providedIn: 'root'
})
export class LogService {
    private injector = inject(Injector);
    private _http: HttpClient | null = null;
    private config = inject<LogConfig>(LOG_CONFIG);
    private storageService = inject(StorageService);
    private logBuffer: any[] = [];
    private flushTimer: any;

    private get http(): HttpClient {
        if (!this._http) {
            const backend = this.injector.get(HttpBackend);
            this._http = new HttpClient(backend);
        }
        return this._http;
    }

    log(level: LogLevel, message: string, context: string, metaData?: any, error?: any): void {
        if (this.config.enabled === false) {
            return;
        }

        const isBrowser = typeof window !== 'undefined';
        const currentHref = isBrowser ? window.location.href : '';
        const currentPathname = isBrowser ? window.location.pathname : '';
        const currentSearch = isBrowser ? window.location.search : '';

        const fullUrl = metaData?.fullUrl || currentHref;
        let urlObj: URL | null = null;
        if (fullUrl) {
            try {
                urlObj = new URL(fullUrl);
            } catch { /* fallback */ }
        }

        // Mapeia para o LogCustomModel esperado pelo Backend (PASCAL CASE)
        const logEntry: any = {
            AppName: this.config.appName,
            PodName: isBrowser ? 'browser' : 'server',
            Tipo: metaData?.tipo || 'log',
            Timestamp: new Date().toISOString(),
            Logs: [`[${level.toUpperCase()}] [${context}] ${message}`],
            Scheme: urlObj?.protocol.replace(':', '') || 'http',
            Host: urlObj?.hostname,
            Port: urlObj?.port ? parseInt(urlObj.port, 10) : (urlObj?.protocol === 'https:' ? 443 : 80),
            Path: urlObj?.pathname || metaData?.path || currentPathname,
            Query: urlObj?.search || currentSearch || '',
            Url: (urlObj?.pathname || currentPathname) + (urlObj?.search || currentSearch || ''),
            FullUrl: fullUrl,
            RelativePath: urlObj?.pathname || metaData?.path || currentPathname,
            Fragment: urlObj?.hash || '',
            Method: metaData?.method || error?.method || 'N/A',
            StatusCode: metaData?.statusCode || error?.status || 0,
            DurationMs: metaData?.durationMs || 0,
            RequestHeaders: metaData?.headers || {},
            ResponseHeaders: metaData?.responseHeaders || {},
            TraceId: metaData?.traceId || error?.traceId || this.getTraceId(),
            UserId: this.storageService.getItem('USUARIO-ID') || undefined,
            StatusCodeProcessamento: metaData?.statusCodeProcessamento || (level === 'Error' || level === 'Critical' ? 'ERROR' : 'SUCCESS'),
            StatusMessageProcessamento: metaData?.statusMessageProcessamento || message || ''
        };

        if (metaData?.body) {
            const sanitized = this.sanitize(metaData.body);
            logEntry.Body = typeof sanitized === 'string' ? this.tryParseJson(sanitized) : sanitized;
        }

        if (metaData?.response || error?.error) {
            const resp = metaData?.response || error?.error;
            const sanitizedResp = this.sanitize(resp);
            logEntry.Response = typeof sanitizedResp === 'string' ? this.tryParseJson(sanitizedResp) : sanitizedResp;
        }

        if (error) {
            logEntry.StackTrace = typeof error === 'string' ? error : JSON.stringify(error, null, 2);
            if (error.message && !message.includes(error.message)) {
                logEntry.Logs.push(`ERROR MESSAGE: ${error.message}`);
            }
        }

        // 1. Console (Debugging Local)
        if (this.config.enableConsole) {
            this.printConsole(level, {
                message,
                context,
                level,
                timestamp: logEntry.Timestamp,
                application: logEntry.AppName,
                environment: this.config.environment
            } as SystemLog);
        }

        // 2. Bufferização para Batching
        this.logBuffer.push(logEntry);

        // Se for erro, envia imediatamente (flush)
        if (level === 'Error' || level === 'Critical') {
            this.flush();
            return;
        }

        // Se atingir o tamanho do lote, envia
        const batchSize = this.config.batchSize || 5;
        if (this.logBuffer.length >= batchSize) {
            this.flush();
            return;
        }

        // Caso contrário, garante que o timer de flush está rodando
        this.startFlushTimer();
    }

    private startFlushTimer() {
        if (this.flushTimer) return;

        const interval = this.config.batchInterval || 3000;
        this.flushTimer = setTimeout(() => this.flush(), interval);
    }

    private flush() {
        if (this.flushTimer) {
            clearTimeout(this.flushTimer);
            this.flushTimer = null;
        }

        if (this.logBuffer.length === 0) return;

        const batch = [...this.logBuffer];
        this.logBuffer = [];

        if (this.config.apiUrl) {
            // Envia o array (batch) se houver mais de um log, ou apenas o objeto se houver um só
            const payload = batch.length === 1 ? batch[0] : batch;
            this.http.post(this.config.apiUrl, payload)
                .pipe(catchError((err: any) => of(null)))
                .subscribe();
        }
    }

    info(message: string, context: string, data?: any) {
        this.log('Info', message, context, data);
    }

    warn(message: string, context: string, data?: any) {
        this.log('Warning', message, context, data);
    }

    error(message: string, context: string, errorOrMeta?: any, error?: any) {
        // Se errorOrMeta tiver propriedades de log (tipo, traceId), tratamos como metaData
        if (errorOrMeta && (errorOrMeta.traceId || errorOrMeta.tipo)) {
            this.log('Error', message, context, errorOrMeta, error);
        } else {
            this.log('Error', message, context, undefined, errorOrMeta);
        }
    }

    private printConsole(level: LogLevel, entry: SystemLog) {
        const prefix = `[${level.toUpperCase()}] [${entry.context}]`;
        const color = this.getColor(level);
        console.log(`%c${prefix} ${entry.message}`, `color: ${color}`, entry.stackTrace || '');
    }


    private getColor(level: LogLevel): string {
        switch (level) {
            case 'Error': return 'red';
            case 'Warning': return 'orange';
            case 'Info': return 'blue';
            case 'Debug': return 'gray';
            default: return 'black';
        }
    }

    private sanitize(data: any): any {

        if (!data) return data;

        try {
            let json = typeof data === 'string' ? data : JSON.stringify(data);
            const keys = this.config.keysToObfuscate || ['password', 'senha', 'token', 'secret', 'key', 'senhaAcesso'];

            keys.forEach(key => {
                const pattern = new RegExp(`("${key}"\\s*:\\s*")([^"]*)(")`, 'gi');
                json = json.replace(pattern, '$1***$3');
            });

            return typeof data === 'string' ? json : JSON.parse(json);
        } catch {
            return data;
        }
    }
    private tryParseJson(data: string): any {
        try {
            return JSON.parse(data);
        } catch {
            return data;
        }
    }

    private sessionTraceId: string | null = null;
    private getTraceId(): string {
        if (!this.sessionTraceId) {
            this.sessionTraceId = Math.random().toString(16).substring(2, 18);
        }
        return this.sessionTraceId;
    }
}
