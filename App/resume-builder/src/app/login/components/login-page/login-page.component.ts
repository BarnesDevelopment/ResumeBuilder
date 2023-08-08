import { Component } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { LoginService } from '../../services/login.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss'],
})
export class LoginPageComponent {
  constructor(
    private titleService: Title,
    private service: LoginService,
    private router: Router,
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
          this.router.navigate(['/']);
        });
    }
  }
}
