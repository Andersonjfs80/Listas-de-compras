import { NgModule, provideBrowserGlobalErrorListeners, ErrorHandler } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { provideHttpClient, withInterceptorsFromDi, withFetch, HTTP_INTERCEPTORS } from '@angular/common/http';

// Material Modules
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { Toolbar } from './layout/toolbar/toolbar';
import { BottomNavComponent } from './layout/bottom-nav/bottom-nav';
import { Welcome } from './modulos/welcome/welcome';

// Lib Logs & Config
import { ModuleHeaderInterceptor, TimeoutInterceptor, LOG_CONFIG, HttpLoggingInterceptor, EncryptionInterceptor, GlobalErrorHandler } from '@app/logs';
import { environment } from '../environments/environment';

@NgModule({
  declarations: [
    AppComponent,
    Toolbar,
    BottomNavComponent,
    Welcome
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    CommonModule,
    FormsModule,
    AppRoutingModule,
    MatToolbarModule,
    MatIconModule,
    MatButtonModule,
    MatSidenavModule
  ],
  providers: [
    provideBrowserGlobalErrorListeners(),
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

