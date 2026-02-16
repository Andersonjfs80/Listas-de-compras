import { InjectionToken } from '@angular/core';

export type LogLevel = 'Trace' | 'Debug' | 'Info' | 'Warning' | 'Error' | 'Critical';

export interface LogConfig {
    apiUrl: string;       // Endpoint do Gateway (ex: http://localhost:5000/api/logs)
    appName: string;      // Nome do Módulo (ex: 'AuthModule')
    environment: string;  // Ambiente (ex: 'Development')
    enableConsole: boolean; // Se deve também imprimir no console do navegador
    keysToObfuscate?: string[]; // Lista de chaves para ofuscar (ex: ['password', 'cpf'])
}

export const LOG_CONFIG = new InjectionToken<LogConfig>('LOG_CONFIG');

/**
 * Interface padronizada de Log do Sistema
 * Segue o padrão utilizado no Backend para integração via Kafka
 */
export interface SystemLog {
    timestamp: string;
    level: LogLevel;
    message: string;
    context?: string;       // Componente ou Serviço de onde veio o log
    userId?: string;        // ID do usuário, se logado
    traceId?: string;       // ID de rastreamento para Distributed Tracing
    stackTrace?: string;    // Pilha de erro em caso de exceptions
    extraData?: any;        // Dados adicionais (JSON)
    application: string;    // Nome do Micro-Frontend (ex: 'App-Home', 'App-Auth')
    environment: string;    // 'Development' | 'Production'
}
