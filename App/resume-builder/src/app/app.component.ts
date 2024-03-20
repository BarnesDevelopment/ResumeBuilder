import { Component, OnInit } from '@angular/core';
import { authConfig } from './services/auth/models/auth-config';
import { OAuthService } from 'angular-oauth2-oidc';

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

  public configure() {
    this.oauthService.configure(authConfig);
    this.oauthService.setupAutomaticSilentRefresh();
    this.oauthService.loadDiscoveryDocumentAndTryLogin();
  }

  get isLoggedIn() {
    return this.oauthService.getIdToken();
  }

  handleLoginClick = () =>
    this.isLoggedIn
      ? this.oauthService.logOut()
      : this.oauthService.initCodeFlow();

  get claims() {
    return this.oauthService.getIdentityClaims() as any;
  }
}
