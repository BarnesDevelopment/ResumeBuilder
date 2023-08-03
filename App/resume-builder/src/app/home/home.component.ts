import { Component } from '@angular/core';
import { ButtonStyle } from '../common/button/button.component';
import { User } from '../models/User';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent {
  user: User = {
    fullName: 'Test Name',
    email: 'test@test.com',
    firstName: 'Test',
    lastName: 'Name',
    username: 'testUser',
    id: '12345678-1234-1234-123456789012',
  };
  useUser = true;

  protected readonly ButtonStyle = ButtonStyle;

  SwitchUser() {
    this.useUser = !this.useUser;
  }
}
