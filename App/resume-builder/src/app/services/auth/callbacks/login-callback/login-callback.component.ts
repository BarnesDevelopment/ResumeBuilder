import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from '../../authentication.service';

@Component({
  selector: 'app-login-callback',
  templateUrl: './login-callback.component.html',
  styleUrls: ['./login-callback.component.scss'],
})
export class LoginCallbackComponent implements OnInit {
  constructor(
    private readonly _authService: AuthenticationService,
    private readonly _router: Router,
  ) {}

  ngOnInit() {
    this._router.navigate(['/'], { replaceUrl: true });
  }
}
