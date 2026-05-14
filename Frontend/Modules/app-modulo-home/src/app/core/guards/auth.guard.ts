import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
    const authService = inject(AuthService);
    const router = inject(Router);
    const platformId = inject(PLATFORM_ID);

    if (authService.isAuthenticated()) {
        console.log('✅ Usuário autenticado, permitindo acesso');
        return true;
    }

    console.warn('⚠️ Usuário não autenticado, redirecionando para login');
    
    if (isPlatformBrowser(platformId)) {
        // Redireciona para o módulo de autenticação configurado no Gateway apenas no navegador
        window.location.href = '/autenticacao';
    }
    
    return false;
};
