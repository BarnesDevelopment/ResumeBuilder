import { Component } from '@angular/core';
import { BorderStyle, ButtonStyle, ButtonComponent } from '../button/button.component';
import { faCaretDown } from '@fortawesome/free-solid-svg-icons';
import { Router } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';
import { FaIconComponent } from '@fortawesome/angular-fontawesome';

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html',
    styleUrls: ['./header.component.scss'],
    standalone: true,
    imports: [FaIconComponent, ButtonComponent],
})
export class HeaderComponent {
  faCaretDown = faCaretDown;
  showUserPanel: boolean;

  protected readonly ButtonStyle = ButtonStyle;
  protected readonly BorderStyle = BorderStyle;

  constructor(
    private router: Router,
    private oauthService: OAuthService,
  ) {}

  ToggleUserPanel() {
    this.showUserPanel = !this.showUserPanel;
  } //TODO: Add functionality to close on mouseout of blur

  GoTo(url: string) {
    this.router.navigate([url]);
  }

  get Claims() {
    return this.oauthService.getIdentityClaims() as any;
  }

  get isLoggedIn() {
    return this.oauthService.getIdToken();
  }

  Login() {
    this.oauthService.initCodeFlow();
  }

  Logout() {
    this.oauthService.logOut();
  }
}
