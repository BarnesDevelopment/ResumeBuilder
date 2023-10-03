import { Component, OnInit } from '@angular/core';
import { ResumeTreeNode } from '../../../models/Resume';
import { Router } from '@angular/router';
import { ResumeService } from '../../services/resume.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';

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
      this.loading = false;
      this.form = new FormGroup({
        title: new FormControl(this.resume.content, [Validators.required]),
        comments: new FormControl(this.resume.comments),
      });
    });
  }

  onSubmit(): void {
    console.log(this.form.value, this.form.valid);
  }
}
