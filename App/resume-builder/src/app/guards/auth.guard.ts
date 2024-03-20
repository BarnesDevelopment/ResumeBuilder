import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { AuthenticationService } from '../services/auth/authentication.service';

export const AuthGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthenticationService);
  if (authService.isLoggedIn) {
    return true;
  }

  authService.logIn();
  return false;
};
