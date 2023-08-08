import { Component, Input, OnInit } from '@angular/core';
import { User } from '../../models/User';
import { BorderStyle, ButtonStyle } from '../button/button.component';
import { faCaretDown } from '@fortawesome/free-solid-svg-icons';
import { Router } from '@angular/router';
import { LoginService } from '../../login/services/login.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent implements OnInit {
  user: User;
  faCaretDown = faCaretDown;
  showUserPanel: boolean;

  protected readonly ButtonStyle = ButtonStyle;
  protected readonly BorderStyle = BorderStyle;

  constructor(
    private router: Router,
    private login: LoginService,
  ) {}

  ngOnInit() {
    const cookie = this.login.getCookie();
    this.login
      .getUser({
        key: cookie.cookie,
        userId: cookie.userId,
        expiration: new Date(),
      })
      .subscribe((x) => {
        this.user = x;
      });
  }

  ToggleUserPanel() {
    this.showUserPanel = !this.showUserPanel;
  } //TODO: Add functionality to close on mouseout of blur

  GoTo(url: string) {
    this.router.navigate([url]);
  }

  Logout() {
    console.log('logout');
  } //TODO: Add logout functionality
}
