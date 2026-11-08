import { ErrorHandler, Injectable, Injector, inject } from '@angular/core';
import { LogService } from '../log.service';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
    private injector = inject(Injector);

    handleError(error: any): void {
        const logService = this.injector.get(LogService);
        
        const message = error.message ? error.message : error.toString();
        const stackTrace = error.stack ? error.stack : undefined;

        // Loga o erro no servidor via LogService e, internamente, ele também loga no console se enableConsole for true
        logService.error(
            `[FRONTEND_EXCEPTION] ${message}`, 
            'GlobalErrorHandler', 
            { tipo: 'exception', error: message, stackTrace },
            error
        );
    }
}
