import { Component, OnInit } from '@angular/core';
import { ResumeTreeNode, SectionType } from '../../../models/Resume';
import { Router } from '@angular/router';
import { ResumeService } from '../../services/resume.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Guid } from 'guid-typescript';
import {
  BorderStyle,
  ButtonStyle,
} from '../../../common/button/button.component';

@Component({
  selector: 'app-edit-resume',
  templateUrl: './edit-resume.component.html',
  styleUrls: ['./edit-resume.component.scss'],
})
export class EditResumeComponent implements OnInit {
  resume: ResumeTreeNode;
  loading: boolean = true;
  form: FormGroup;

  constructor(
    private router: Router,
    private service: ResumeService,
  ) {}

  ngOnInit(): void {
    this.service.getResume(this.router.url.split('/')[2]).subscribe((res) => {
      this.resume = res;
      this.form = new FormGroup({
        title: new FormControl(this.resume.content, [Validators.required]),
        comments: new FormControl(this.resume.comments),
      });
      this.loading = false;
      console.log(this.resume, this.form);
    });
  }

  addSection(): void {
    this.form.addControl(
      `section${this.resume.children.length}title`,
      new FormControl('', [Validators.required]),
    );

    this.resume.children.push({
      id: Guid.create(),
      content: 'Section Title',
      comments: null,
      children: [],
      active: true,
      userId: this.resume.userId,
      sectionType: SectionType.Section,
      parentId: this.resume.id,
      order: this.resume.children.length,
      depth: 1,
    });

    console.log(this.resume, this.form);
  }

  protected readonly ButtonStyle = ButtonStyle;
  protected readonly BorderStyle = BorderStyle;
}
