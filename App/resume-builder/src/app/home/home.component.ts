import { Component, inject, OnInit } from '@angular/core';
import {
  BorderStyle,
  ButtonComponent,
  ButtonStyle,
} from '../common/button/button.component';
import { ResumeHeader } from '../models/Resume';
import { ResumeService } from '../resume/services/resume.service';
import { OAuthService } from 'angular-oauth2-oidc';
import { MatCardModule } from '@angular/material/card';
import { LoginSplashScreenComponent } from '../common/login-splash-screen/login-splash-screen.component';
import { Guid } from 'guid-typescript';
import { MatDialog } from '@angular/material/dialog';
import { UpdateTitleComponent } from '../resume/components/update-title/update-title.component';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  standalone: true,
  imports: [LoginSplashScreenComponent, MatCardModule, ButtonComponent],
})
export class HomeComponent implements OnInit {
  private readonly service = inject(ResumeService);
  private readonly oauthService = inject(OAuthService);
  private readonly dialog = inject(MatDialog);
  protected readonly ButtonStyle = ButtonStyle;
  protected readonly BorderStyle = BorderStyle;
  isLoading = true;
  resumes: ResumeHeader[] = [];
  next: number = 0;

  ngOnInit(): void {
    if (this.isLoggedIn) this.getResumes();
  }

  getResumes() {
    this.service.getResumes().subscribe(resumes => {
      this.resumes = resumes;
      this.next = this.resumes.length;
      this.isLoading = false;
    });
  }

  get isLoggedIn() {
    return this.oauthService.getIdToken();
  }

  queueDelete(resume: ResumeHeader) {
    this.service.deleteNode(resume.id).subscribe(() => {
      this.resumes = this.resumes.filter(r => r.id !== resume.id);
    });
  }

  duplicate(id: Guid) {
    this.dialog.open(UpdateTitleComponent, {
      data: { id, next: this.next },
      disableClose: true,
      width: '25rem',
      height: '15rem',
    });
  }
}
