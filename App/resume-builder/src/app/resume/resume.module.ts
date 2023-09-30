import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CreateResumeComponent } from './components/create-resume/create-resume.component';
import {ReactiveFormsModule} from "@angular/forms";
import {MatInputModule} from "@angular/material/input";
import {MatButtonModule} from "@angular/material/button";

@NgModule({
  declarations: [
    CreateResumeComponent
  ],
  imports: [CommonModule, ReactiveFormsModule, MatInputModule, MatButtonModule],
})
export class ResumeModule {}
