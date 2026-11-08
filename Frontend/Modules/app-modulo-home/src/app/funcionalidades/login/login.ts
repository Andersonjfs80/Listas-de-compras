import { Component, HostListener, Inject, PLATFORM_ID, OnInit, NgZone } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Router } from '@angular/router';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { LogService, StorageService } from '@app/logs';

@Component({
    selector: 'app-login-wrapper',
    standalone: true,
    imports: [CommonModule],
    template: `
      <iframe [src]="authUrl" style="width: 100vw; height: 100vh; border: none; margin: 0; padding: 0; display: block; position: fixed; top: 0; left: 0;"></iframe>
    `
})
export class LoginWrapperComponent implements OnInit {
    authUrl: SafeResourceUrl;

    constructor(
        private router: Router,
        private sanitizer: DomSanitizer,
        private ngZone: NgZone,
        private logService: LogService,
        private storageService: StorageService,
        @Inject(PLATFORM_ID) private platformId: Object
    ) {
        // Confia na URL do Pod de Autenticação (Porta 4204)
        this.authUrl = this.sanitizer.bypassSecurityTrustResourceUrl('http://localhost:4204/login');
    }

    ngOnInit() {
        if (isPlatformBrowser(this.platformId)) {
            if (this.storageService.getItem('token')) {
                this.logService.info('[LoginWrapperComponent] Token encontrado no ngOnInit, redirecionando para a Home.', 'LoginWrapperComponent');
                this.router.navigate(['/']);
            }
        }
    }

    @HostListener('window:message', ['$event'])
    onMessage(event: MessageEvent) {
        if (isPlatformBrowser(this.platformId)) {
            // Segurança: Garantir que a mensagem vem de origens esperadas e possui o tipo correto
            if (event.data && event.data.type === 'AUTH_SUCCESS') {
                this.logService.info('[LoginWrapperComponent] Token MFE recebido com sucesso via PostMessage!', 'LoginWrapperComponent');

                this.ngZone.run(() => {
                    if (event.data.token) {
                        this.storageService.setItem('token', event.data.token);
                        this.logService.info(`[LoginWrapperComponent] Token salvo com sucesso no storage: ${event.data.token.substring(0, 15)}...`, 'LoginWrapperComponent');
                    } else {
                        this.logService.error('[LoginWrapperComponent] O postMessage chegou sem Token válido!', 'LoginWrapperComponent', event.data);
                    }

                    if (event.data.usuario) {
                        this.storageService.setItem('user', JSON.stringify(event.data.usuario));
                    }

                    this.logService.info('[LoginWrapperComponent] Disparando router.navigate para a Home (/).', 'LoginWrapperComponent');
                    this.router.navigate(['/']).then(success => {
                        this.logService.info(`[LoginWrapperComponent] Resultado do navigate para a Home: ${success}`, 'LoginWrapperComponent');
                    }).catch(err => {
                        this.logService.error('[LoginWrapperComponent] Erro interno no navigate para a Home.', 'LoginWrapperComponent', err);
                    });
                });
            }
        }
    }
}
