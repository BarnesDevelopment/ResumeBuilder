import { Component, Input } from '@angular/core';
import { User } from '../../models/User';
import { BorderStyle, ButtonStyle } from '../button/button.component';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent {
  @Input() user: User | undefined;
  protected readonly ButtonStyle = ButtonStyle;
  protected readonly BorderStyle = BorderStyle;
}
