import { Component, Inject, inject, OnInit } from '@angular/core';
import { ResumeService } from '../../services/resume.service';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Guid } from 'guid-typescript';
import { ResumeTreeNode } from '../../../models/Resume';
import { InputComponent } from '../../../common/input/input.component';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import {
  ButtonComponent,
  ButtonStyle,
} from '../../../common/button/button.component';
import { Router } from '@angular/router';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-update-title',
  standalone: true,
  imports: [
    InputComponent,
    ReactiveFormsModule,
    ButtonComponent,
    MatProgressSpinnerModule,
  ],
  templateUrl: './update-title.component.html',
  styleUrl: './update-title.component.scss',
})
export class UpdateTitleComponent implements OnInit {
  private readonly service = inject(ResumeService);
  private readonly router = inject(Router);
  protected readonly ButtonStyle = ButtonStyle;
  isLoading: boolean = true;
  saveDisabled: boolean = true;
  resume: ResumeTreeNode;
  form: FormGroup;
  titleError: string;

  constructor(
    public dialogRef: MatDialogRef<UpdateTitleComponent>,
    @Inject(MAT_DIALOG_DATA)
    public data: {
      id: Guid;
      next: number;
    },
  ) {}

  ngOnInit(): void {
    this.service.getResume(this.data.id.toString()).subscribe(resume => {
      this.resume = resume;
      this.form = new FormGroup({
        title: new FormControl(this.resume.content, [
          Validators.required,
          Validators.pattern(/^(\s+\S+\s*)*(?!\s).*$/),
        ]),
        comments: new FormControl(this.resume.comments),
      });
      this.form.valueChanges.subscribe(() => {
        this.validateForm();
      });
      this.isLoading = false;
    });
  }

  validateForm() {
    const invalidForm = this.form.invalid;
    const titleMatch =
      this.form.get('title').value.trim() === this.resume.content.trim();

    if (titleMatch) {
      this.form.get('title').setErrors({ titleMatch: true });
    }

    if (
      this.form.get('title').hasError('required') ||
      this.form.get('title').hasError('pattern')
    ) {
      this.titleError = 'Title cannot be blank';
    } else if (this.form.get('title').hasError('titleMatch')) {
      this.titleError = 'Title must be different';
    } else {
      this.titleError = '';
    }

    this.form.markAllAsTouched();
    this.form.updateValueAndValidity();

    this.saveDisabled = invalidForm || titleMatch;
  }

  save() {
    this.validateForm();
    if (this.saveDisabled) {
      return;
    }
    this.service.duplicateResume(this.data.id.toString()).subscribe(id => {
      this.service.getResume(id).subscribe(resume => {
        resume.content = this.form.get('title').value;
        resume.comments = this.form.get('comments').value;
        resume.order = this.data.next;
        this.service.updateResume([resume]).subscribe(() => {
          console.log('updated');
          this.router.navigate(['/edit', id]);
          this.dialogRef.close();
        });
      });
    });
  }

  cancel() {
    this.dialogRef.close();
  }
}
