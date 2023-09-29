import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ResumeHeader, ResumeTreeNode } from '../../models/Resume';
import { Observable } from 'rxjs';
import { environment } from '../../../environment/environment';
import { LoginService } from '../../login/services/login.service';

@Injectable({
  providedIn: 'root',
})
export class ResumeService {
  env = environment;

  constructor(
    private http: HttpClient,
    private loginService: LoginService,
  ) {}

  public getResumes(): Observable<ResumeHeader[]> {
    const cookie = this.loginService.getCookie();
    const headers = new HttpHeaders().set('Authorization', cookie.cookie);
    return this.http.get<ResumeHeader[]>(
      `${this.env.apiBasePath}/resume/get-all`,
      {
        headers,
      },
    );
  }

  public createResume(resume: ResumeTreeNode): Observable<ResumeTreeNode> {
    const cookie = this.loginService.getCookie();
    const headers = new HttpHeaders().set('Authorization', cookie.cookie);
    return this.http.post<ResumeTreeNode>(
      `${this.env.apiBasePath}/resume/create`,
      resume,
      {
        headers,
      },
    );
  }
}
