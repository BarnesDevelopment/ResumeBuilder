import { Component } from '@angular/core';
import { ButtonStyle } from '../common/button/button.component';
import { environment } from '../../environment/environment';
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent {
  protected readonly ButtonStyle = ButtonStyle;
  protected readonly environment = environment;
}
