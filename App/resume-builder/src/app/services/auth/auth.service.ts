import { Injectable } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { DemoService } from './demo.service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private loggedIn: boolean = false;
  private demo: boolean = false;

  constructor(
    private oauthService: OAuthService,
    private demoService: DemoService,
    private router: Router,
  ) {}

  public isLoggedIn() {
    return this.loggedIn;
  }

  public getClaims() {
    if (this.loggedIn && this.demo) {
      return { name: 'Demo User' };
    } else {
      return this.oauthService.getIdentityClaims() as any;
    }
  }

  public login(demo: boolean = false) {
    if (demo) {
      this.demoService.login().subscribe(() => {
        this.loggedIn = true;
        this.demo = true;
        this.router.navigate(['/login-callback']);
      });
    } else {
      this.oauthService.initCodeFlow();
    }
  }

  public logout() {
    if (this.demo) {
      this.demoService.logout().subscribe(() => {
        this.loggedIn = false;
        this.demo = false;
        this.router.navigate(['/']);
      });
    } else {
      this.oauthService.logOut();
    }
  }
}
