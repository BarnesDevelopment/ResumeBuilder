import { Component, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { BorderStyle, ButtonStyle } from '../common/button/button.component';
import { environment } from '../../environment/environment';
import { ResumeHeader } from '../models/Resume';
import { ResumeService } from '../resume/services/resume.service';
import { OAuthService } from 'angular-oauth2-oidc';
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  protected readonly ButtonStyle = ButtonStyle;
  protected readonly BorderStyle = BorderStyle;
  isLoading = true;
  resumes: ResumeHeader[] = [];
  next: number = 0;

  constructor(
    private service: ResumeService,
    private oauthService: OAuthService,
  ) {}

  ngOnInit(): void {
    setTimeout(() => {
      if (this.isLoggedIn) this.getResumes();
    }, 2000);
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
}
