import { Component } from '@angular/core';
import { AuthenticationService } from './services/auth/authentication.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  title = 'resume-builder';

  constructor(private authService: AuthenticationService) {
    authService.configure();
  }

  isLoggedIn = this.authService.isLoggedIn;
  handleLoginClick = () =>
    this.authService.isLoggedIn
      ? this.authService.logOut()
      : this.authService.logIn();

  claims = this.authService.claims;
}
