import { inject, Injectable } from '@angular/core';
import { DemoService } from './demo.service';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { FusionAuthService } from '@fusionauth/angular-sdk-custom';
import { from, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly fusionAuthService: FusionAuthService =
    inject(FusionAuthService);
  // private readonly oauthService: OAuthService = inject(OAuthService);
  private readonly demoService: DemoService = inject(DemoService);
  private readonly router: Router = inject(Router);
  private readonly cookieService: CookieService = inject(CookieService);
  private demo: boolean = false;

  public isLoggedIn() {
    return this.fusionAuthService.isLoggedIn();
  }

  public isDemo() {
    return this.demo;
  }

  public getClaims() {
    if (this.isLoggedIn() && this.demo) {
      return of({ name: 'Demo User' });
    } else {
      // return this.oauthService.getIdentityClaims() as any;
      return from(this.fusionAuthService.getUserInfo());
    }
  }

  public login(demo: boolean = false) {
    if (demo) {
      this.demoService.login().subscribe(token => {
        this.demo = true;
        this.cookieService.set('demo-token', token);
        this.router.navigate(['/login-callback']);
      });
    } else {
      this.fusionAuthService.startLogin();
    }
  }

  public logout() {
    if (this.demo) {
      this.demoService.logout().subscribe(() => {
        this.demo = false;
        this.cookieService.delete('demo-token');
        this.router.navigate(['/']);
      });
    } else {
      this.fusionAuthService.logout();
    }
  }
}
