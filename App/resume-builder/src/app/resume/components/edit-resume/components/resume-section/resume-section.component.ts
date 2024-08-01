import { Component, Input, OnInit } from '@angular/core';
import {
  newResumeTreeNode,
  NodeType,
  ResumeTreeNode,
  SectionDisplayType,
  SectionDisplayTypeList,
} from '../../../../../models/Resume';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { InputComponent } from '../../../../../common/input/input.component';
import { SectionListComponent } from '../section-list/section-list.component';
import { SectionParagraphComponent } from '../section-paragraph/section-paragraph.component';
import { SectionEducationComponent } from '../section-education/section-education.component';
import { SectionWorkExperienceComponent } from '../section-work-experience/section-work-experience.component';

@Component({
  selector: 'app-resume-section',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    InputComponent,
    SectionListComponent,
    SectionParagraphComponent,
    SectionEducationComponent,
    SectionWorkExperienceComponent,
  ],
  templateUrl: './resume-section.component.html',
  styleUrl: './resume-section.component.scss',
})
export class ResumeSectionComponent implements OnInit {
  @Input() section: ResumeTreeNode;

  form: FormGroup;

  ngOnInit() {
    this.form = new FormGroup({
      name: new FormControl(this.section.content),
      type: new FormControl(''),
    });
    if (this.section.children.length > 0) {
      this.form.controls['type'].setValue(this.section.children[0].nodeType);
    }
    this.form.controls['type'].valueChanges.subscribe(value => {
      this.UpdateSectionType(value);
    });
  }

  UpdateSectionType(value: SectionDisplayType) {
    switch (value) {
      case SectionDisplayType.List:
        this.section.children = [
          newResumeTreeNode(NodeType.List, 0, this.section),
        ];
        break;
      case SectionDisplayType.Education:
        this.section.children = [
          newResumeTreeNode(NodeType.Education, 0, this.section),
        ];
        break;
      case SectionDisplayType.Paragraph:
        this.section.children = [
          newResumeTreeNode(NodeType.Paragraph, 0, this.section),
        ];
        break;
      case SectionDisplayType.WorkExperience:
        this.section.children = [
          newResumeTreeNode(NodeType.WorkExperience, 0, this.section),
        ];
        break;
      default:
        this.section.children = [];
    }
  }

  protected readonly SectionDisplayType = SectionDisplayType;
  protected readonly SectionDisplayTypeList = SectionDisplayTypeList;
}
