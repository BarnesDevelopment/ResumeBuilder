import { Injectable } from '@angular/core';
import { User, UserManager, WebStorageStateStore } from 'oidc-client';
import { UserManagerSettings } from './models/userManagerSettings';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {
  isUserDefined = false;
  private _user: User | null;
  private _userManager: UserManager;
  isLoggedIn() {
    return this._user != null && !this._user.expired;
  }

  getAccessToken() {
    return this._user ? this._user.access_token : '';
  }

  getClaims() {
    return this._user?.profile;
  }

  startAuthentication(): Promise<void> {
    this.getUserManager();
    return this._userManager.signinRedirect();
  }

  completeAuthentication() {
    this.getUserManager();
    return this._userManager.signinRedirectCallback().then(user => {
      this._user = user;
      this.isUserDefined = true;
    });
  }

  startLogout(): Promise<void> {
    this.getUserManager();
    return this._userManager.signoutRedirect();
  }

  completeLogout() {
    this.getUserManager();
    this._user = null;
    return this._userManager.signoutRedirectCallback();
  }

  silentSignInAuthentication() {
    this.getUserManager();
    return this._userManager.signinSilentCallback();
  }

  private getUserManager() {
    if (!this._userManager) {
      const userManagerSettings: UserManagerSettings =
        new UserManagerSettings();

      // set up settings
      userManagerSettings.authority = '';
      userManagerSettings.client_id = '';
      userManagerSettings.response_type = 'code';
      userManagerSettings.scope = 'openid profile resume-id';

      userManagerSettings.redirect_uri = 'http://localhost:4200/auth-callback';
      userManagerSettings.post_logout_redirect_uri =
        'http://localhost:4200/logout-callback';

      userManagerSettings.automaticSilentRenew = true;
      userManagerSettings.silent_redirect_uri =
        'http://localhost:4200/silent-callback';

      userManagerSettings.userStore = new WebStorageStateStore({
        store: window.localStorage,
      });

      this._userManager = new UserManager(userManagerSettings);

      this._userManager.getUser().then(user => {
        if (user) {
          this._user = user;
          this.isUserDefined = true;
        }
      });
    }
  }
}
