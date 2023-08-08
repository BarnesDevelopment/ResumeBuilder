import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
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

  public login(username: string, password: string): Observable<User> {
    const headers = new HttpHeaders()
      .set('username', username)
      .set('key', password);
    return new Observable((observer) => {
      this.http
        .get<Cookie>(`${this.env.apiBasePath}/users/login`, {
          headers: headers,
        })
        .subscribe((cookie) => {
          this.addCookie(cookie);
          this.getUser(cookie).subscribe((user) => {
            observer.next(user);
            observer.complete();
          });
        });
    });
  }

  private getUser(cookie: Cookie): Observable<User> {
    const headers = new HttpHeaders().set('Authorization', cookie.key);
    return this.http.get<User>(
      `${this.env.apiBasePath}/users/user?userId=${cookie.userId}`,
      {
        headers: headers,
      },
    );
  }

  private addCookie(cookie: Cookie): Cookie {
    this.cookieService.set(`resume-builder-cookie`, cookie.key, {
      expires: new Date(cookie.expiration),
      sameSite: 'Strict',
      path: '/',
      domain: this.env.domain,
      secure: true,
    });
    return cookie;
  }

  public getCookie(): string {
    return this.cookieService.get('resume-builder-cookie');
  }
}
