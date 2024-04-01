import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
@Component({
    selector: 'app-login-callback',
    templateUrl: './login-callback.component.html',
    styleUrls: ['./login-callback.component.scss'],
    standalone: true,
})
export class LoginCallbackComponent implements OnInit {
  constructor(private readonly _router: Router) {}

  ngOnInit() {
    this._router.navigate(['/'], { replaceUrl: true });
  }
}
