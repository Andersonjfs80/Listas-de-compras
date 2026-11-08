import { NgModule, ErrorHandler } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { provideHttpClient, withInterceptorsFromDi, withFetch, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ModuleHeaderInterceptor, TimeoutInterceptor, LOG_CONFIG, HttpLoggingInterceptor, EncryptionInterceptor, GlobalErrorHandler } from '@app/logs';
import { environment } from '../environments/environment';

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
