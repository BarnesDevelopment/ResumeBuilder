import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Cookie } from '../../models/Cookie';
import { environment } from '../../../environment/environment';
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
  env = environment;

  public login(username: string, password: string): Observable<any> {
    const headers = new HttpHeaders()
      .set('username', username)
      .set('key', password);
    return this.http.get(`${this.env.apiBasePath}/users/login`, {
      headers,
    });
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
}
