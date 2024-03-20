import { Component } from '@angular/core';
@Component({
  selector: 'app-silent-callback',
  templateUrl: './silent-callback.component.html',
  styleUrls: ['./silent-callback.component.scss'],
})
export class SilentCallbackComponent {
  constructor() {}

  ngOnInit() {
    // this._authService.silentSignInAuthentication();
  }
}
