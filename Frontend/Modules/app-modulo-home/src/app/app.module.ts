import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { provideHttpClient, withInterceptorsFromDi, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ModuleHeaderInterceptor, TimeoutInterceptor, LOG_CONFIG } from '@app/logs';

@NgModule({
    declarations: [
        AppComponent
    ],
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        AppRoutingModule
    ],
    providers: [
        provideHttpClient(withInterceptorsFromDi()),
        { provide: HTTP_INTERCEPTORS, useClass: ModuleHeaderInterceptor, multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: TimeoutInterceptor, multi: true },
        {
            provide: LOG_CONFIG,
            useValue: {
                appName: 'APP-HOME',
                apiUrl: 'https://localhost/app-api-autenticacao',
                environment: 'Development',
                enableConsole: true
            }
        }
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
