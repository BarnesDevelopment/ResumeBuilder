import { Injectable } from '@angular/core';
import {
  User,
  UserManager,
  UserManagerSettings,
  WebStorageStateStore,
} from 'oidc-client';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {
  private _user: User | null;
  private _userManager: UserManager;

  constructor() {
    this._userManager = new UserManager(this.getUserManagerSettings());

    this._userManager.getUser().then(user => {
      this._user = user;
    });
  }
  isLoggedIn() {
    // this._user = JSON.parse(sessionStorage.getItem('oidc.user')) as User;
    return this._user != null && !this._user.expired;
  }

  getAccessToken() {
    return this._user ? this._user.access_token : '';
  }

  getClaims() {
    return this._user?.profile;
  }

  startAuthentication(): Promise<void> {
    return this._userManager.signinRedirect();
  }

  completeAuthentication() {
    return this._userManager.signinRedirectCallback().then(user => {
      debugger;
      this._user = user;
    });
  }

  startLogout(): Promise<void> {
    return this._userManager.signoutRedirect();
  }

  completeLogout() {
    this._user = null;
    return this._userManager.signoutRedirectCallback();
  }

  silentSignInAuthentication() {
    return this._userManager.signinSilentCallback();
  }

  private getUserManagerSettings(): UserManagerSettings {
    return {
      authority: 'https://auth.barnes7619.com/realms/ResumeBuilder/',
      client_id: 'resume-builder',
      redirect_uri: 'https://localhost:4200/login-callback',
      post_logout_redirect_uri: 'https://localhost:4200/logout-callback',
      response_type: 'code',
      response_mode: 'query',
      scope: 'profile email roles resume-id',
      silent_redirect_uri: 'https://localhost:4200/silent-callback',
      automaticSilentRenew: true,
      filterProtocolClaims: true,
      loadUserInfo: false,
    };
  }
}
