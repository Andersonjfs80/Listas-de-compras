import { inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { LogService } from '@app/logs';

export const authGuard: CanActivateFn = (route, state) => {
    const authService = inject(AuthService);
    const router = inject(Router);
    const platformId = inject(PLATFORM_ID);
    const logService = inject(LogService);

    // No servidor (SSR), o localStorage não existe.
    // Liberamos a rota para permitir a renderização inicial do esqueleto HTML no Express.
    // A validação real de autenticação e redirecionamento será re-executada no navegador (cliente).
    if (!isPlatformBrowser(platformId)) {
        return true;
    }

    const isAuth = authService.isAuthenticated();

    if (isAuth) {
        logService.info(`[authGuard] Usuário autenticado. Acesso liberado para a rota: ${state.url}`, 'authGuard');
        return true;
    }

    logService.warn(`[authGuard] Tentativa de acesso não autorizado bloqueada para a rota: ${state.url}`, 'authGuard');

    if (isPlatformBrowser(platformId)) {
        logService.info('[authGuard] Redirecionando usuário não autenticado para /login', 'authGuard');
        return router.parseUrl('/login');
    }
    
    return false;
};
