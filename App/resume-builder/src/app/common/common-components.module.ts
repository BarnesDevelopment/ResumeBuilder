import { NgModule } from '@angular/core';
import { HeaderComponent } from './header/header.component';
import { NgIf } from '@angular/common';
import { ButtonComponent } from './button/button.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { LoginSplashScreenComponent } from './login-splash-screen/login-splash-screen.component';
import { InputComponent } from './input/input.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
    exports: [
        HeaderComponent,
        ButtonComponent,
        LoginSplashScreenComponent,
        InputComponent,
    ],
    imports: [
        NgIf,
        FontAwesomeModule,
        FormsModule,
        FormsModule,
        ReactiveFormsModule,
        HeaderComponent,
        ButtonComponent,
        LoginSplashScreenComponent,
        InputComponent,
    ],
})
export class CommonComponentsModule {}
