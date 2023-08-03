import { Component, Input } from '@angular/core';
import { User } from '../../models/User';
import { BorderStyle, ButtonStyle } from '../button/button.component';
import { faCaretDown } from '@fortawesome/free-solid-svg-icons';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent {
  @Input() user: User | undefined;
  faCaretDown = faCaretDown;
  showUserPanel: boolean = false;

  protected readonly ButtonStyle = ButtonStyle;
  protected readonly BorderStyle = BorderStyle;

  constructor(private router: Router) {}

  ToggleUserPanel() {
    this.showUserPanel = !this.showUserPanel;
  } //TODO: Add functionality to close on mouseout of blur

  GoToHome() {
    this.router.navigate(['/']);
  }
}
