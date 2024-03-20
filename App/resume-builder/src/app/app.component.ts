import { Component } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { authConfig } from './services/auth/models/auth-config';
import { JwksValidationHandler } from 'angular-oauth2-oidc-jwks';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  title = 'resume-builder';

  constructor(private oauthService: OAuthService) {
    this.configure();
  }

  private configure() {
    this.oauthService.configure(authConfig);
    this.oauthService.tokenValidationHandler = new JwksValidationHandler();
    this.oauthService.loadDiscoveryDocumentAndTryLogin();
  }

  get isLoggedIn() {
    console.log(this.oauthService.getIdToken());
    return this.oauthService.getIdToken();
  }

  get claims() {
    return this.oauthService.getIdentityClaims() as any;
  }

  handleLoginClick = () =>
    this.isLoggedIn
      ? this.oauthService.logOut()
      : this.oauthService.initLoginFlow();
}
