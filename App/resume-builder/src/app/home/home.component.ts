import { Component } from '@angular/core';
import { BorderStyle, ButtonStyle } from '../common/button/button.component';
import { environment } from '../../environment/environment';
import { ResumeHeader } from '../models/Resume';
import { Guid } from 'guid-typescript';
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent {
  protected readonly ButtonStyle = ButtonStyle;
  protected readonly BorderStyle = BorderStyle;
  environment = environment;
  resumes: ResumeHeader[] = [
    {
      title: 'Resume 1',
      comments: 'This is my first resume',
      id: Guid.create(),
    },
    {
      title: 'Resume 2',
      comments: 'This is my second resume',
      id: Guid.create(),
    },
    {
      title: 'Resume 3',
      comments: 'This is my third resume',
      id: Guid.create(),
    },
  ];
}
