import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { provideHttpClient, withInterceptorsFromDi, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app';
import { ModuleHeaderInterceptor, TimeoutInterceptor, LOG_CONFIG } from '@app/logs';

// Material Modules
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCheckboxModule } from '@angular/material/checkbox';

// Components
import { LoginComponent } from './features/login/login';
import { CadastroComponent } from './features/cadastro/cadastro';
import { RecuperarSenhaComponent } from './features/recuperar-senha/recuperar-senha';
import { RedefinirSenhaComponent } from './features/redefinir-senha/redefinir-senha';

@NgModule({
    declarations: [
        AppComponent,
        LoginComponent,
        CadastroComponent,
        RecuperarSenhaComponent,
        RedefinirSenhaComponent
    ],
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        AppRoutingModule,
        FormsModule,
        MatCardModule,
        MatFormFieldModule,
        MatInputModule,
        MatButtonModule,
        MatIconModule,
        MatProgressSpinnerModule,
        MatCheckboxModule
    ],
    providers: [
        provideHttpClient(withInterceptorsFromDi()),
        { provide: HTTP_INTERCEPTORS, useClass: ModuleHeaderInterceptor, multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: TimeoutInterceptor, multi: true },
        {
            provide: LOG_CONFIG,
            useValue: {
                appName: 'APP-AUTH',
                apiUrl: 'http://localhost:5005/app-api-autenticacao',
                environment: 'Development',
                enableConsole: true
            }
        }
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
