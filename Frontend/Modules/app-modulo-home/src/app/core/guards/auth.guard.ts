import { Injectable, inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
    const authService = inject(AuthService);
    const router = inject(Router);

    if (authService.isAuthenticated()) {
        console.log('✅ Usuário autenticado, permitindo acesso');
        return true;
    }

    console.warn('⚠️ Usuário não autenticado, redirecionando para login');
    // Redireciona para o módulo de autenticação configurado no Gateway
    window.location.href = '/autenticacao';
    return false;
};
