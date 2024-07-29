import { Component, Input, OnInit } from '@angular/core';
import {
  ResumeTreeNode,
  SectionDisplayType,
  SectionDisplayTypeList,
} from '../../../../../models/Resume';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { InputComponent } from '../../../../../common/input/input.component';

@Component({
  selector: 'app-resume-section',
  standalone: true,
  imports: [ReactiveFormsModule, InputComponent],
  templateUrl: './resume-section.component.html',
  styleUrl: './resume-section.component.scss',
})
export class ResumeSectionComponent implements OnInit {
  @Input() section: ResumeTreeNode;

  form: FormGroup;

  ngOnInit() {
    this.form = new FormGroup({
      name: new FormControl(this.section.content),
      type: new FormControl(this.section.nodeType),
    });
  }

  protected readonly SectionDisplayType = SectionDisplayType;
  protected readonly SectionDisplayTypeList = SectionDisplayTypeList;
}
