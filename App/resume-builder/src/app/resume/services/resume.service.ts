import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  newResumeTreeNodeJson,
  ResumeHeader,
  ResumeTreeNode,
} from '../../models/Resume';
import { Observable } from 'rxjs';
import { environment } from '../../../environment/environment';
import { OAuthService } from 'angular-oauth2-oidc';

@Injectable({
  providedIn: 'root',
})
export class ResumeService {
  env = environment;

  constructor(
    private http: HttpClient,
    private oauthService: OAuthService,
  ) {}

  public getResumes(): Observable<ResumeHeader[]> {
    return this.http.get<ResumeHeader[]>(
      `${this.env.apiBasePath}/resume/get-all`,
    );
  }

  public getResume(id: string): Observable<ResumeTreeNode> {
    return this.http.get<ResumeTreeNode>(
      `${this.env.apiBasePath}/resume/get/${id}`,
    );
  }

  public updateResume(resume: ResumeTreeNode): Observable<ResumeTreeNode> {
    return this.http.post<ResumeTreeNode>(
      `${this.env.apiBasePath}/resume/upsert`,
      newResumeTreeNodeJson(resume),
    );
  }

  public deleteNode(resume: ResumeTreeNode): Observable<boolean> {
    return this.http.delete<boolean>(
      `${this.env.apiBasePath}/resume/delete/${resume.id}`,
    );
  }
}
