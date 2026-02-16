import { Injectable, Inject, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { SystemLog, LogLevel, LOG_CONFIG } from './log.models';
import type { LogConfig } from './log.models';
import { catchError } from 'rxjs/operators';
import { of } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class LogService {
    private http = inject(HttpClient);
    private config = inject<LogConfig>(LOG_CONFIG);

    constructor() { }

    /**
     * Envia um log estruturado para o Backend (que jogará no Kafka)
     */
    log(level: LogLevel, message: string, context: string, extraData?: any, error?: any): void {
        const logEntry: SystemLog = {
            timestamp: new Date().toISOString(),
            level: level,
            message: message,
            context: context,
            application: this.config.appName,
            environment: this.config.environment,
            extraData: extraData ? JSON.stringify(this.sanitize(extraData)) : undefined,
            stackTrace: error ? (error.stack || JSON.stringify(error)) : undefined
            // userId e traceId seriam pegos de um SessionService / HttpInterceptor
        };

        // 1. Console (Debugging Local)
        if (this.config.enableConsole) {
            this.printConsole(level, logEntry);
        }

        // 2. HTTP (Backend -> Kafka) - Fire and Forget (não bloqueia UI)
        this.http.post(this.config.apiUrl, logEntry)
            .pipe(catchError((err: any) => of(null))) // Silencia falhas de log para não quebrar app
            .subscribe();
    }

    info(message: string, context: string, data?: any) {
        this.log('Info', message, context, data);
    }

    warn(message: string, context: string, data?: any) {
        this.log('Warning', message, context, data);
    }

    error(message: string, context: string, error?: any) {
        this.log('Error', message, context, undefined, error);
    }

    private printConsole(level: LogLevel, entry: SystemLog) {
        const prefix = `[${level.toUpperCase()}] [${entry.context}]`;
        const color = this.getColor(level);
        console.log(`%c${prefix} ${entry.message}`, `color: ${color}`, entry.extraData || '', entry.stackTrace || '');
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
            let json = JSON.stringify(data);
            const keys = this.config.keysToObfuscate || ['password', 'senha', 'token', 'email', 'cartao'];

            keys.forEach(key => {
                // Regex para encontrar "chave": "valor"
                // Grupos: 1: "chave": ", 2: valor (qualquer coisa exceto aspas), 3: "
                const pattern = new RegExp(`("${key}"\\s*:\\s*")([^"]*)(")`, 'gi');
                json = json.replace(pattern, '$1***$3');
            });

            return JSON.parse(json);
        } catch {
            return data;
        }
    }
}
