import { Component } from '@angular/core';
import {
  BorderStyle,
  ButtonComponent,
  ButtonStyle,
} from '../button/button.component';
import { faCaretDown } from '@fortawesome/free-solid-svg-icons';
import { Router } from '@angular/router';
import { FaIconComponent } from '@fortawesome/angular-fontawesome';
import { AuthService } from '../../services/auth/auth.service';

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
    private authService: AuthService,
  ) {}

  ToggleUserPanel() {
    this.showUserPanel = !this.showUserPanel;
  } //TODO: Add functionality to close on mouseout of blur

  GoTo(url: string) {
    this.router.navigate([url]);
  }

  get Claims() {
    return this.authService.getClaims();
  }

  get isLoggedIn() {
    return this.authService.isLoggedIn();
  }

  Login() {
    this.authService.login();
  }

  Logout() {
    this.authService.logout();
  }
}
