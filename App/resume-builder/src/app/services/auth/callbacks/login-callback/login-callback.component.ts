import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from '../../authentication.service';

@Component({
  selector: 'app-login-callback',
  templateUrl: './login-callback.component.html',
  styleUrls: ['./login-callback.component.scss'],
})
export class LoginCallbackComponent {
  constructor(
    private readonly _authService: AuthenticationService,
    private readonly _router: Router,
  ) {}

  ngOnInit() {
    // this._authService.completeAuthentication();
    // this._router.navigate(['/']);
  }
}
