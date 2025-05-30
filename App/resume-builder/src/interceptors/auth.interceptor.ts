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
import { OAuthService } from 'angular-oauth2-oidc';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(
    private readonly _authService: OAuthService,
    private readonly _router: Router,
  ) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler,
  ): Observable<HttpEvent<any>> {
    const accessToken = this._authService.getAccessToken();
    const headers = request.headers;
    const setHeaders = {};
    if (accessToken) {
      setHeaders['Authorization'] = `Bearer ${accessToken}`;
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
