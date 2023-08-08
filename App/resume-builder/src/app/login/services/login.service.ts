import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
} from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Cookie } from '../../models/Cookie';
import { Environment } from '../../../environment/environment';
import { CookieService } from 'ngx-cookie-service';
import { User } from '../../models/User';

@Injectable({
  providedIn: 'root',
})
export class LoginService {
  constructor(
    private http: HttpClient,
    private cookieService: CookieService,
  ) {}
  env = Environment;

  public login(username: string, password: string): Observable<any> {
    const headers = new HttpHeaders()
      .set('username', username)
      .set('key', password);
    return this.http
      .get(`${this.env.apiBasePath}/users/login`, {
        headers,
      })
      .pipe(
        catchError((error) => {
          let errorMsg: string;
          if (error.error instanceof ErrorEvent) {
            errorMsg = `Error: ${error.error.message}`;
          } else {
            errorMsg = this.getServerErrorMessage(error);
          }

          return throwError(errorMsg);
        }),
      );
  }

  public getUser(cookie: Cookie): Observable<User> {
    const headers = new HttpHeaders().set('Authorization', cookie.key);
    return this.http.get<User>(
      `${this.env.apiBasePath}/users/user?userId=${cookie.userId}`,
      {
        headers: headers,
      },
    );
  }

  public addCookie(cookie: Cookie): Cookie {
    this.cookieService.set(`resume-builder-cookie`, cookie.key, {
      expires: new Date(cookie.expiration),
      sameSite: 'Strict',
      path: '/',
      domain: this.env.domain,
      secure: true,
    });

    this.cookieService.set(`resume-builder-userid`, cookie.userId, {
      expires: new Date(cookie.expiration),
      sameSite: 'Strict',
      path: '/',
      domain: this.env.domain,
      secure: true,
    });
    return cookie;
  }

  public logout() {
    this.cookieService.delete(`resume-builder-cookie`);
    this.cookieService.delete(`resume-builder-userid`);
  }

  public getCookie() {
    return {
      cookie: this.cookieService.get('resume-builder-cookie'),
      userId: this.cookieService.get('resume-builder-userid'),
    };
  }

  private getServerErrorMessage(error: HttpErrorResponse): string {
    switch (error.status) {
      case 401: {
        return `Unauthorized: ${error.message}`;
      }
      case 404: {
        return `Not Found: ${error.message}`;
      }
      case 403: {
        return `Access Denied: ${error.message}`;
      }
      case 500: {
        return `Internal Server Error: ${error.message}`;
      }
      default: {
        return `Unknown Server Error: ${error.message}`;
      }
    }
  }
}
