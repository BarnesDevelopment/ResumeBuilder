import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-logout-callback',
  templateUrl: './logout-callback.component.html',
  styleUrls: ['./logout-callback.component.scss'],
  standalone: true,
})
export class LogoutCallbackComponent {
  constructor(private readonly _router: Router) {}
}
