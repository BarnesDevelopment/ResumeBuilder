import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  duplicateTreeNode,
  ResumeHeader,
  ResumeTreeNode,
} from '../../models/Resume';
import { catchError, Observable, of } from 'rxjs';
import { environment } from '../../../environment/environment';
import { Guid } from 'guid-typescript';

@Injectable({
  providedIn: 'root',
})
export class ResumeService {
  env = environment;

  constructor(private http: HttpClient) {}

  public getResumes(): Observable<ResumeHeader[]> {
    return this.http.get<ResumeHeader[]>(`${this.env.apiBasePath}/get-all`);
  }

  public getResume(id: string): Observable<ResumeTreeNode> {
    return this.http.get<ResumeTreeNode>(`${this.env.apiBasePath}/get/${id}`);
  }

  public updateResume(resumes: ResumeTreeNode[]): Observable<void> {
    return this.http.post<void>(
      `${this.env.apiBasePath}/upsert`,
      resumes.map(duplicateTreeNode),
    );
  }

  public duplicateResume(id: string): Observable<string> {
    return this.http.post<string>(
      `${this.env.apiBasePath}/duplicate/${id}`,
      {},
    );
  }

  public deleteNode(guid: Guid): Observable<boolean> {
    return this.http
      .delete<boolean>(`${this.env.apiBasePath}/delete/${guid}`)
      .pipe(
        catchError(error => {
          if (error.status === 404) {
            return of(true);
          } else {
            throw error;
          }
        }),
      );
  }

  public getPreview(resume: ResumeTreeNode): Observable<Blob> {
    return this.http.get(`${this.env.apiBasePath}/build/${resume.id}`, {
      responseType: 'blob',
    });
  }
}
