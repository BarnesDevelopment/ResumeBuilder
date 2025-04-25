import { Component, inject } from '@angular/core';
import {
  BorderStyle,
  ButtonComponent,
  ButtonStyle,
} from '../button/button.component';
import { AuthService } from '../../services/auth/auth.service';

@Component({
  selector: 'app-login-splash-screen',
  templateUrl: './login-splash-screen.component.html',
  styleUrls: ['./login-splash-screen.component.scss'],
  standalone: true,
  imports: [ButtonComponent],
})
export class LoginSplashScreenComponent {
  protected readonly ButtonStyle = ButtonStyle;
  protected readonly BorderStyle = BorderStyle;
  private readonly authService = inject(AuthService);

  login() {
    this.authService.login();
  }

  demo() {
    this.authService.login(true);
  }
}
