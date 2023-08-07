import { Component } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { LoginService } from '../../services/login.service';

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss'],
})
export class LoginPageComponent {
  constructor(
    private titleService: Title,
    private service: LoginService,
  ) {
    this.titleService.setTitle('Login');
  }
  hidePassword = true;
  protected readonly console = console;

  username = new FormControl('', [Validators.required]);
  password = new FormControl('', [Validators.required]);

  loginInfo = new FormGroup({
    username: this.username,
    password: this.password,
  });

  onSubmit() {
    if (this.username.valid && this.password.valid) {
      this.service
        .login(this.username.value, this.password.value)
        .subscribe((x) => {
          console.log(x);
        });
    }
  }
}
