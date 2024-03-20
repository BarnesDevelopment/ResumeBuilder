import { Component } from '@angular/core';
import { AuthenticationService } from '../../authentication.service';

@Component({
  selector: 'app-silent-callback',
  templateUrl: './silent-callback.component.html',
  styleUrls: ['./silent-callback.component.scss'],
})
export class SilentCallbackComponent {
  constructor(private readonly _authService: AuthenticationService) {}

  ngOnInit() {
    // this._authService.silentSignInAuthentication();
  }
}
