import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-logout-callback',
  templateUrl: './logout-callback.component.html',
  styleUrls: ['./logout-callback.component.scss'],
})
export class LogoutCallbackComponent implements OnInit {
  constructor(private readonly _router: Router) {}

  ngOnInit() {
    // this._authService.completeLogout();
    // this._router.navigate(['/']);
  }
}
