import { Injectable } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { DemoService } from './demo.service';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';

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
    private cookieService: CookieService,
  ) {}

  public isLoggedIn() {
    this.checkForCookie();
    return this.loggedIn;
  }

  public getClaims() {
    if (this.loggedIn && this.demo) {
      return { name: 'Demo User' };
    } else {
      return this.oauthService.getIdentityClaims() as any;
    }
  }

  private checkForCookie() {
    const cookie = this.cookieService.get('resume-id');
    this.loggedIn = cookie !== '';
    this.demo = cookie !== '';
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
        this.cookieService.delete('resume-id');
        this.router.navigate(['/']);
      });
    } else {
      this.oauthService.logOut();
    }
  }
}
