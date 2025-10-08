import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { CommonComponentsModule } from './common/common-components.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ResumeModule } from './resume/resume.module';
import { LoginCallbackComponent } from './services/auth/callbacks/login-callback/login-callback.component';
import { LogoutCallbackComponent } from './services/auth/callbacks/logout-callback/logout-callback.component';
import { SilentCallbackComponent } from './services/auth/callbacks/silent-callback/silent-callback.component';
import { AuthInterceptor } from '../interceptors/auth.interceptor';
import { OAuthModule } from 'angular-oauth2-oidc';
import { MatDialogModule } from '@angular/material/dialog';
import { FusionAuthModule } from '@fusionauth/angular-sdk';
import {environment} from "../environment/environment";

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    AppRoutingModule,
    CommonComponentsModule,
    FontAwesomeModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    HttpClientModule,
    MatCardModule,
    FormsModule,
    ResumeModule,
    OAuthModule.forRoot(),
    HomeComponent,
    PageNotFoundComponent,
    LoginCallbackComponent,
    LogoutCallbackComponent,
    SilentCallbackComponent,
    MatDialogModule,
    FusionAuthModule.forRoot({
      clientId: '', // Your FusionAuth client ID
      serverUrl: 'auth.barnes7619.com', // The base URL of the server that performs the token exchange
      redirectUri: environment.fusionAuthRedirectUri, // The URI that the user is directed to after the login/register/logout action
      shouldAutoRefresh: true // option to configure the SDK to automatically handle token refresh. Defaults to false if not specified here.
    }),
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true,
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
