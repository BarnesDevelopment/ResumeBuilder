import { Component, Input, OnChanges, output } from '@angular/core';
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
import { Guid } from 'guid-typescript';
import { UpsertSignal } from '../upsert-signal/upsert-signal';
import { ButtonComponent } from '../../../../../common/button/button.component';

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
    ButtonComponent,
  ],
  templateUrl: './resume-section.component.html',
  styleUrl: './resume-section.component.scss',
})
export class ResumeSectionComponent extends UpsertSignal implements OnChanges {
  @Input() section: ResumeTreeNode;

  form: FormGroup;

  ngOnChanges() {
    this.form = new FormGroup({
      name: new FormControl(this.section.content),
      type: new FormControl(''),
    });
    if (this.section.children.length > 0) {
      this.form.controls['type'].setValue(this.section.children[0].nodeType);
    }
    this.form.controls['name'].valueChanges.subscribe(value => {
      this.section.content = value;
      this.queueSave(this.section);
    });
    this.form.controls['type'].valueChanges.subscribe(value => {
      this.UpdateSectionType(value);
    });
  }

  UpdateSectionType(value: SectionDisplayType) {
    const shouldDelete = this.section.children.length > 0;
    if (shouldDelete) {
      this.queueDelete(this.section.children[0].id);
    }
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
    if (this.section.children.length > 0) {
      this.queueSave(this.section.children[0]);
    }
  }

  protected readonly SectionDisplayType = SectionDisplayType;
  protected readonly SectionDisplayTypeList = SectionDisplayTypeList;

  RemoveItem($index: number) {
    this.queueDelete(this.section.children[$index].id);
    this.section.children.splice($index, 1);
    this.ReorderChildren();
  }

  AddItem() {
    switch (this.form.controls['type'].value) {
      case SectionDisplayType.Education:
        this.section.children.push(
          newResumeTreeNode(
            NodeType.Education,
            this.section.children.length,
            this.section,
          ),
        );
        break;
      case SectionDisplayType.WorkExperience:
        this.section.children.push(
          newResumeTreeNode(
            NodeType.WorkExperience,
            this.section.children.length,
            this.section,
          ),
        );
        break;
    }
    this.queueSave(this.section.children[this.section.children.length - 1]);
  }

  ReorderChildren() {
    this.section.children.forEach((child, index) => {
      child.order = index;
      this.queueSave(child);
    });
  }
}
