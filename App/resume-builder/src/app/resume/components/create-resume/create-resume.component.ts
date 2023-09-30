import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ResumeService } from '../../services/resume.service';
import { ResumeTreeNode, SectionType } from '../../../models/Resume';
import { Guid } from 'guid-typescript';

@Component({
  selector: 'app-create-resume',
  templateUrl: './create-resume.component.html',
  styleUrls: ['./create-resume.component.scss'],
})
export class CreateResumeComponent {
  form: FormGroup = new FormGroup(
    {
      title: new FormControl('', [
        Validators.required,
        Validators.maxLength(150),
        Validators.minLength(3),
      ]),
      comments: new FormControl('', [Validators.maxLength(500)]),
    }
  );

  constructor(private service: ResumeService) {}

  onSubmit() {
    console.log(this.form.value);
    const resume: ResumeTreeNode = {
      content: this.form.controls['title'].value,
      comments: this.form.controls['comments'].value,
      depth: 0,
      id: Guid.create(),
      order: 0,
      sectionType: SectionType.Resume,
      parentId: null,
      userId: null,
      active: true,
      children: [],
    };

    this.service.createResume(resume).subscribe((res) => {
      console.log(res);
    });
  }
}
