import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginModule } from './login/login.module';
import { HomeComponent } from './home/home.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { CommonComponentsModule } from './common/common-components.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {ResumeModule} from "./resume/resume.module";
import { LoginCallbackComponent } from './services/auth/callbacks/login-callback/login-callback.component';
import { LogoutCallbackComponent } from './services/auth/callbacks/logout-callback/logout-callback.component';
import { SilentCallbackComponent } from './services/auth/callbacks/silent-callback/silent-callback.component';

@NgModule({
  declarations: [AppComponent, HomeComponent, PageNotFoundComponent, LoginCallbackComponent, LogoutCallbackComponent, SilentCallbackComponent],
  imports: [
    BrowserModule,
    AppRoutingModule,
    LoginModule,
    CommonComponentsModule,
    FontAwesomeModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    HttpClientModule,
    MatCardModule,
    FormsModule,
    ResumeModule,
  ],
  providers: [
    // {
    //   provide: HTTP_INTERCEPTORS,
    //   useClass: CustomErrorHttpInterceptor,
    //   multi: true,
    // },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
