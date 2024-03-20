import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
export const AuthGuard: CanActivateFn = (route, state) => {
  const authService = inject(OAuthService);
  if (authService.hasValidAccessToken()) {
    return true;
  }

  authService.initLoginFlow();
  return false;
};
