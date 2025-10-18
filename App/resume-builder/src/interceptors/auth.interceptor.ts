import { Injectable } from '@angular/core';
import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { AuthService } from '../app/services/auth/auth.service';
import { CookieService } from 'ngx-cookie-service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(
    private readonly _authService: AuthService,
    private readonly _router: Router,
    private readonly _cookieService: CookieService,
  ) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler,
  ): Observable<HttpEvent<any>> {
    const headers = request.headers;
    const setHeaders = {};

    if (this._authService.isDemo()) {
      var token = this._cookieService.get('demo-token');

      setHeaders['Authorization'] = `Bearer ${token}`;
    }
    const authRequest = request.clone({
      headers,
      withCredentials: true,
      setHeaders,
    });

    return next.handle(authRequest).pipe(
      tap({
        error: error => {
          const respError = error as HttpErrorResponse;
          if (respError.status === 401 || respError.status === 403) {
            this._router.navigate(['/unauthorized']);
          }
        },
      }),
    );
  }
}
