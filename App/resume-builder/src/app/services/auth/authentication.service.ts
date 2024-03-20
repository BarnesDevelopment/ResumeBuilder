import { Injectable } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { authConfig } from './models/auth-config';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {
  constructor(private oauthService: OAuthService) {}

  public configure() {
    this.oauthService.configure(authConfig);
    this.oauthService.loadDiscoveryDocumentAndTryLogin();
  }

  get isLoggedIn() {
    return this.oauthService.getIdToken();
  }

  get accessToken() {
    return this.oauthService.getAccessToken();
  }

  get claims() {
    return this.oauthService.getIdentityClaims() as any;
  }

  public logIn() {
    this.oauthService.initCodeFlow();
  }

  public logOut() {
    this.oauthService.revokeTokenAndLogout();
  }
}
