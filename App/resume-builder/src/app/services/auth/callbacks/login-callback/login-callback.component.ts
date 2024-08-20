import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-login-callback',
  templateUrl: './login-callback.component.html',
  styleUrls: ['./login-callback.component.scss'],
  standalone: true,
  imports: [MatProgressSpinnerModule],
})
export class LoginCallbackComponent implements OnInit {
  constructor(private readonly _router: Router) {}

  loadingText: string = 'Logging in   ';
  dots = 0;

  ngOnInit() {
    setInterval(() => {
      if (this.dots === 0) {
        this.dots++;
        this.loadingText = 'Logging in.  ';
      } else if (this.dots === 1) {
        this.dots++;
        this.loadingText = 'Logging in.. ';
      } else if (this.dots === 2) {
        this.dots++;
        this.loadingText = 'Logging in...';
      } else {
        this.dots = 0;
        this.loadingText = 'Logging in   ';
      }
    }, 200);
    setTimeout(() => {
      this._router.navigate(['/'], { replaceUrl: true });
    }, 2000);
  }
}
