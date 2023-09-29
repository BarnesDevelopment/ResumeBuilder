import { Component } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { LoginService } from '../../services/login.service';
import { Router } from '@angular/router';
import { Cookie } from '../../../models/Cookie';

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
  badLogin = false;

  username = new FormControl('', {
    validators: [Validators.required],
  });
  password = new FormControl('', {
    validators: [Validators.required],
  });

  loginInfo = new FormGroup({
    username: this.username,
    password: this.password,
  });

  onSubmit() {
    this.badLogin = false;
    this.username.setErrors(null);
    this.password.setErrors(null);
    if (this.username.valid && this.password.valid) {
      this.service.login(this.username.value, this.password.value).subscribe({
        next: (cookie: Cookie) => {
          this.service.addCookie(cookie);
        },
        error: () => {
          this.username.setErrors(['badLogin']);
          this.password.setErrors(['badLogin']);
          this.badLogin = true;
        },
        complete: () => {
          console.log('complete');
          if (this.badLogin === false) {
            this.router.navigate(['/']).then(() => {
              window.location.reload();
            });
          }
        },
      });
    }
  }
}
