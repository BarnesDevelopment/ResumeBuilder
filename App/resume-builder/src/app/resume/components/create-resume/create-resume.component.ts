import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ResumeService } from '../../services/resume.service';
import { ResumeTreeNode, SectionType } from '../../../models/Resume';
import { Guid } from 'guid-typescript';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-create-resume',
  templateUrl: './create-resume.component.html',
  styleUrls: ['./create-resume.component.scss'],
})
export class CreateResumeComponent {
  form: FormGroup = new FormGroup({
    title: new FormControl('', [
      Validators.required,
      Validators.maxLength(150),
      Validators.minLength(3),
    ]),
    comments: new FormControl('', [Validators.maxLength(500)]),
  });

  constructor(
    private service: ResumeService,
    private router: Router,
    private route: ActivatedRoute,
  ) {}

  onSubmit() {
    console.log(this.form.value);
    this.route.queryParams.subscribe((params) => {
      const resume: ResumeTreeNode = {
        content: this.form.controls['title'].value,
        comments: this.form.controls['comments'].value,
        depth: 0,
        id: Guid.create(),
        order: params['next'],
        sectionType: SectionType.Resume,
        parentId: null,
        userId: Guid.createEmpty(),
        active: true,
        children: [],
      };

      this.service.createResume(resume).subscribe((res) => {
        this.router.navigate(['/edit', res.id]);
      });
    });
  }
}
