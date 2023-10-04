import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ResumeService } from '../../services/resume.service';
import { ResumeTreeNode, SectionType } from '../../../models/Resume';
import { Guid } from 'guid-typescript';
import { ActivatedRoute, Router } from '@angular/router';
import {
  BorderStyle,
  ButtonStyle,
} from '../../../common/button/button.component';

@Component({
  selector: 'app-create-resume',
  templateUrl: './create-resume.component.html',
  styleUrls: ['./create-resume.component.scss'],
})
export class CreateResumeComponent {
  phonePattern =
    '^(\\+\\d{1,2}\\s?)?\\(?\\d{3}\\)?[\\s.-]?\\d{3}[\\s.-]?\\d{4}$';

  form: FormGroup = new FormGroup({
    title: new FormControl('', [
      Validators.required,
      Validators.maxLength(150),
      Validators.minLength(3),
    ]),
    comments: new FormControl('', [Validators.maxLength(500)]),
    name: new FormControl('', [Validators.required]),
    email: new FormControl('', [Validators.required, Validators.email]),
    phone: new FormControl('', [
      Validators.required,
      Validators.pattern(this.phonePattern),
    ]),
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

      this.service.updateResume(resume).subscribe((res) => {
        this.router.navigate(['/edit', res.id]);
      });
    });
  }

  protected readonly ButtonStyle = ButtonStyle;
  protected readonly BorderStyle = BorderStyle;
}
