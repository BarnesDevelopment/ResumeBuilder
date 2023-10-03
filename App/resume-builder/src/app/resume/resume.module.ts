import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CreateResumeComponent } from './components/create-resume/create-resume.component';
import { ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { EditResumeComponent } from './components/edit-resume/edit-resume.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import {CommonComponentsModule} from "../common/common-components.module";

@NgModule({
  declarations: [CreateResumeComponent, EditResumeComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    CommonComponentsModule,
  ],
})
export class ResumeModule {}
