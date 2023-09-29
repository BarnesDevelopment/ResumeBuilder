import { Component, OnDestroy, OnInit } from '@angular/core';
import { User } from '../../models/User';
import { BorderStyle, ButtonStyle } from '../button/button.component';
import { faCaretDown } from '@fortawesome/free-solid-svg-icons';
import { NavigationEnd, Router } from '@angular/router';
import { LoginService } from '../../login/services/login.service';
import { environment } from '../../../environment/environment';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent implements OnInit, OnDestroy {
  navigationSubscription;
  user: User;
  faCaretDown = faCaretDown;
  showUserPanel: boolean;

  protected readonly ButtonStyle = ButtonStyle;
  protected readonly BorderStyle = BorderStyle;

  constructor(
    private router: Router,
    private login: LoginService,
  ) {
    this.navigationSubscription = this.router.events.subscribe((e: any) => {
      // If it is a NavigationEnd event re-initalise the component
      if (e instanceof NavigationEnd) {
        this.user = null;
      }
    });
  }

  ngOnInit() {
    const cookie = this.login.getCookie();
    if (cookie.cookie !== '' && cookie.userId !== '') {
      this.login
        .getUser({
          key: cookie.cookie,
          userId: cookie.userId,
          expiration: new Date(),
        })
        .subscribe((x) => {
          this.user = x;
          environment.loggedIn = true;
        });
    } else {
      this.user = null;
      environment.loggedIn = false;
    }
  }

  ngOnDestroy() {
    if (this.navigationSubscription) {
      this.navigationSubscription.unsubscribe();
    }
  }

  ToggleUserPanel() {
    this.showUserPanel = !this.showUserPanel;
  } //TODO: Add functionality to close on mouseout of blur

  GoTo(url: string) {
    this.router.navigate([url]);
  }

  Logout() {
    this.login.logout();
    this.showUserPanel = false;
    this.router.navigate(['/']);
    environment.loggedIn = false;
  }
}
