import { Component } from '@angular/core';
import { AuthenticationService } from '../../authentication.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-logout-callback',
  templateUrl: './logout-callback.component.html',
  styleUrls: ['./logout-callback.component.scss'],
})
export class LogoutCallbackComponent {
  constructor(
    private readonly _authService: AuthenticationService,
    private readonly _router: Router,
  ) {}

  ngOnInit() {
    // this._authService.completeLogout();
    // this._router.navigate(['/']);
  }
}
