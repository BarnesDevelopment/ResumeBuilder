import { Injectable } from '@angular/core';
import { environment } from '../../../environment/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DemoService {
  env = environment;

  constructor(private http: HttpClient) {}

  public login(): Observable<void> {
    return this.http.put<void>(`${this.env.apiBasePath}/demo/login`, {});
  }

  public logout(): Observable<void> {
    return this.http.delete<void>(`${this.env.apiBasePath}/demo/logout`);
  }
}
