import { Component, Inject, inject, OnInit } from "@angular/core";
import { ResumeService } from "../../services/resume.service";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { Guid } from "guid-typescript";
import { ResumeTreeNode } from "../../../models/Resume";
import { InputComponent } from "../../../common/input/input.component";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { ButtonComponent, ButtonStyle } from "../../../common/button/button.component";
import { Router } from "@angular/router";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";

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
  isLoading: boolean = true;
  resume: ResumeTreeNode;
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<UpdateTitleComponent>,
    @Inject(MAT_DIALOG_DATA)
    public data: {
      id: Guid;
      order: number;
    },
  ) {}

  ngOnInit(): void {
    this.service.getResume(this.data.id.toString()).subscribe(resume => {
      this.resume = resume;
      this.form = new FormGroup({
        title: new FormControl(this.resume.content),
        comments: new FormControl(this.resume.comments),
      });
      this.isLoading = false;
    });
  }

  protected readonly ButtonStyle = ButtonStyle;

  save() {
    this.service.duplicateResume(this.data.id.toString()).subscribe(id => {
      this.service.getResume(id).subscribe(resume => {
        resume.content = this.form.get('ti"title"alue;
        resume.comments = this.form.get('co"comments"alue;
        resume.order = this.data.order;
        this.service.updateResume([resume]).subscribe(() => {
          this.router.navigate(['/r"/resume"his.data.id]);
          this.dialogRef.close();
        });
      });
    });
  }
}
