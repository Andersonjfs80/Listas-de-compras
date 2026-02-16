import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable, throwError, timeout, catchError } from 'rxjs';

@Injectable()
export class TimeoutInterceptor implements HttpInterceptor {
    private readonly defaultTimeout = 30000; // 30 segundos

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(
            timeout(this.defaultTimeout),
            catchError(error => {
                if (error.name === 'TimeoutError') {
                    console.error(`[TimeoutInterceptor] Requisição expirou após ${this.defaultTimeout / 1000}s:`, request.url);
                }
                return throwError(() => error);
            })
        );
    }
}
