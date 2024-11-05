import { Component, inject } from '@angular/core';
import {
  BorderStyle,
  ButtonComponent,
  ButtonStyle,
} from '../button/button.component';
import { OAuthService } from 'angular-oauth2-oidc';
import { Router } from '@angular/router';
import { DemoService } from '../../services/auth/demo.service';

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
  private readonly router = inject(Router);
  private readonly demoService = inject(DemoService);

  constructor(private oauthService: OAuthService) {}

  login() {
    this.oauthService.initCodeFlow();
  }

  demo() {
    this.demoService.login().subscribe(() => {
      console.log('Demo login successful');

      this.demoService.logout().subscribe(() => {
        console.log('Demo logout successful');
      });
    });
  }
}
