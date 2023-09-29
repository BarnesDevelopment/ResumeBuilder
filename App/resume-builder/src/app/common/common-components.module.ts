import { NgModule } from '@angular/core';
import { HeaderComponent } from './header/header.component';
import { NgIf } from '@angular/common';
import { ButtonComponent } from './button/button.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { LoginSplashScreenComponent } from './login-splash-screen/login-splash-screen.component';

@NgModule({
  declarations: [HeaderComponent, ButtonComponent, LoginSplashScreenComponent],
  exports: [HeaderComponent, ButtonComponent],
  imports: [NgIf, FontAwesomeModule],
})
export class CommonComponentsModule {}
