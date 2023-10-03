import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import {
  newResumeTreeNodeJson,
  ResumeHeader,
  ResumeTreeNode,
} from '../../models/Resume';
import { Observable } from 'rxjs';
import { environment } from '../../../environment/environment';
import { LoginService } from '../../login/services/login.service';
import { Guid } from 'guid-typescript';

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

  public getResume(id: string): Observable<ResumeTreeNode> {
    const cookie = this.loginService.getCookie();
    const headers = new HttpHeaders().set('Authorization', cookie.cookie);
    return this.http.get<ResumeTreeNode>(
      `${this.env.apiBasePath}/resume/get/${id}`,
      {
        headers,
      },
    );
  }

  public updateResume(resume: ResumeTreeNode): Observable<ResumeTreeNode> {
    const cookie = this.loginService.getCookie();
    const headers = new HttpHeaders().set('Authorization', cookie.cookie);
    return this.http.post<ResumeTreeNode>(
      `${this.env.apiBasePath}/resume/upsert`,
      newResumeTreeNodeJson(resume),
      {
        headers,
      },
    );
  }
}
