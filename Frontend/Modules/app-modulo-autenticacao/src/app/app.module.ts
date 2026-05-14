import { NgModule, ErrorHandler } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { provideHttpClient, withInterceptorsFromDi, withFetch, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app';
import { ModuleHeaderInterceptor, TimeoutInterceptor, LOG_CONFIG, HttpLoggingInterceptor, EncryptionInterceptor, GlobalErrorHandler } from '@app/logs';

// Material Modules
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSnackBarModule } from '@angular/material/snack-bar';

// Components
import { LoginComponent } from './features/login/login';
import { CadastroComponent } from './features/cadastro/cadastro';
import { RecuperarSenhaComponent } from './features/recuperar-senha/recuperar-senha';
import { RedefinirSenhaComponent } from './features/redefinir-senha/redefinir-senha';
import { AlterarSenhaComponent } from './features/alterar-senha/alterar-senha';

import { environment } from '../environments/environment';

@NgModule({
    declarations: [
        AppComponent,
        LoginComponent,
        CadastroComponent,
        RecuperarSenhaComponent,
        RedefinirSenhaComponent,
        AlterarSenhaComponent
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
        MatCheckboxModule,
        MatSnackBarModule
    ],
    providers: [
        provideHttpClient(withInterceptorsFromDi(), withFetch()),
        { provide: HTTP_INTERCEPTORS, useClass: ModuleHeaderInterceptor, multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: TimeoutInterceptor, multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: HttpLoggingInterceptor, multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: EncryptionInterceptor, multi: true },
        { provide: ErrorHandler, useClass: GlobalErrorHandler },
        {
            provide: LOG_CONFIG,
            useValue: {
                appName: environment.appName,
                apiUrl: environment.apiUrls.logs,
                environment: environment.name,
                enableConsole: environment.enableConsole,
                secretKey: environment.secretKey,
                enableBodyEncryption: environment.enableBodyEncryption
            }
        }
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
