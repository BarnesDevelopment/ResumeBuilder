import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { AuthenticationService } from '../app/services/auth/authentication.service';
import { Router } from '@angular/router';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(
    private readonly _authService: AuthenticationService,
    private readonly _router: Router,
  ) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler,
  ): Observable<HttpEvent<any>> {
    const accessToken = this._authService.getAccessToken();
    const headers = request.headers.set(
      'Authorization',
      `Bearer ${accessToken}`,
    );
    const authRequest = request.clone({ headers });

    return next.handle(authRequest).pipe(
      tap({
        error: error => {
          const respError = error as HttpErrorResponse;
          if (respError.status === 401 || respError.status === 403) {
            debugger;
            this._router.navigate(['/unauthorized']);
          }
        },
      }),
    );
  }
}
