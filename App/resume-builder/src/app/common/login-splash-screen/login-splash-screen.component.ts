import { Component } from '@angular/core';
import { BorderStyle, ButtonStyle } from '../button/button.component';

@Component({
  selector: 'app-login-splash-screen',
  templateUrl: './login-splash-screen.component.html',
  styleUrls: ['./login-splash-screen.component.scss'],
})
export class LoginSplashScreenComponent {
  protected readonly ButtonStyle = ButtonStyle;
  protected readonly BorderStyle = BorderStyle;
}
