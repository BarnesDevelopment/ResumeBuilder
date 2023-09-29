import { Component, OnInit } from '@angular/core';
import { BorderStyle, ButtonStyle } from '../common/button/button.component';
import { environment } from '../../environment/environment';
import { ResumeHeader } from '../models/Resume';
import { Guid } from 'guid-typescript';
import { ResumeService } from '../resume/services/resume.service';
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  protected readonly ButtonStyle = ButtonStyle;
  protected readonly BorderStyle = BorderStyle;
  environment = environment;
  resumes: ResumeHeader[] = [];

  constructor(private service: ResumeService) {}

  ngOnInit(): void {
    this.service.getResumes().subscribe((resumes) => {
      this.resumes = resumes;
      console.log(environment.loggedIn);
    });
  }
}
