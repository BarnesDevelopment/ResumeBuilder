import { Component, Input } from '@angular/core';
import {
  NodeType,
  ResumeTreeNode,
  SectionDisplayType,
} from '../../../../../models/Resume';
import {
  BorderStyle,
  ButtonComponent,
  ButtonStyle,
} from '../../../../../common/button/button.component';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { InputComponent } from '../../../../../common/input/input.component';
import { Guid } from 'guid-typescript';
import { ResumeService } from '../../../../services/resume.service';

@Component({
  selector: 'app-section',
  standalone: true,
  imports: [ButtonComponent, FormsModule, InputComponent, ReactiveFormsModule],
  templateUrl: './section.component.html',
  styleUrl: './section.component.scss',
})
export class SectionComponent {
  protected readonly SectionDisplayType = SectionDisplayType;
  protected readonly BorderStyle = BorderStyle;
  protected readonly ButtonStyle = ButtonStyle;

  @Input() section: ResumeTreeNode;
  @Input() form: FormGroup;

  @Input() resume: ResumeTreeNode;

  constructor(private service: ResumeService) {}

  //region SectionType
  sectionTypeChanged(section: ResumeTreeNode) {
    section.children.forEach(node => {
      this.service.deleteNode(node).subscribe();
      this.form.removeControl(`section${section.order}listItem${node.order}`);
    });
    section.children = [];

    switch (this.form.controls[`section${section.order}type`].value) {
      case SectionDisplayType.List:
        this.sectionAddListForm(section);
        break;
      case SectionDisplayType.Paragraph:
        this.sectionAddParagraphForm(section);
        break;
      case SectionDisplayType.Education:
        this.sectionAddEducationForm(section);
        break;
      case SectionDisplayType.WorkExperience:
        this.sectionAddWorkExperienceForm(section);
        break;
      default:
        break;
    }
  }

  //region: Section Add Forms

  //region List
  sectionAddListForm(section: ResumeTreeNode) {
    const index = section.children.length;
    const listId = Guid.create();
    section.children.push({
      id: listId,
      content: '',
      comments: '',
      children: [
        {
          id: Guid.create(),
          content: '',
          comments: '',
          children: [],
          active: true,
          userId: this.resume.userId,
          nodeType: NodeType.ListItem,
          parentId: listId,
          order: 0,
          depth: 3,
        },
      ],
      active: true,
      userId: this.resume.userId,
      nodeType: NodeType.List,
      parentId: section.id,
      order: index,
      depth: 2,
    });

    this.form.addControl(
      `section${section.order}listItem0`,
      new FormControl(section.children[index].children[0].content, [
        Validators.required,
      ]),
    );

    this.form.controls[
      `section${section.order}listItem0`
    ].valueChanges.subscribe(res => {
      section.children[index].children[0].content = res;
    });
  }

  addListItem(sectionOrder: number, list: ResumeTreeNode) {
    const index = list.children.length;
    list.children.push({
      id: Guid.create(),
      content: '',
      comments: '',
      children: [],
      active: true,
      userId: this.resume.userId,
      nodeType: NodeType.ListItem,
      parentId: list.id,
      order: index,
      depth: 3,
    });

    this.form.addControl(
      `section${sectionOrder}listItem${index}`,
      new FormControl(list.children[index].content, [Validators.required]),
    );

    this.form.controls[
      `section${sectionOrder}listItem${index}`
    ].valueChanges.subscribe(res => {
      list.children[index].content = res;
    });
  }

  removeListItem(sectionOrder: number, list: ResumeTreeNode) {
    const nodeToDrop = list.children.pop();
    this.form.removeControl(
      `section${sectionOrder}listItem${list.children.length}`,
    );

    this.service.deleteNode(nodeToDrop).subscribe();
  }

  //endregion
  //region Paragraph
  sectionAddParagraphForm(section: ResumeTreeNode) {
    const index = section.children.length;
    section.children.push({
      id: Guid.create(),
      content: '',
      comments: '',
      children: [],
      active: true,
      userId: this.resume.userId,
      nodeType: NodeType.Paragraph,
      parentId: section.id,
      order: index,
      depth: 2,
    });

    this.form.addControl(
      `section${section.order}paragraph`,
      new FormControl(section.children[index].content, [Validators.required]),
    );

    this.form.controls[
      `section${section.order}paragraph`
    ].valueChanges.subscribe(res => {
      section.children[index].content = res;
    });
  }

  //endregion
  //region Education
  sectionAddEducationForm(section: ResumeTreeNode) {
    const index = section.children.length;
    const educationId = Guid.create();
    section.children.push({
      id: educationId,
      content: '',
      comments: '',
      children: [
        {
          id: Guid.create(),
          content: '',
          comments: '',
          children: [],
          active: true,
          userId: this.resume.userId,
          nodeType: NodeType.ListItem,
          parentId: educationId,
          order: 0,
          depth: 3,
        },
        {
          id: Guid.create(),
          content: '',
          comments: '',
          children: [],
          active: true,
          userId: this.resume.userId,
          nodeType: NodeType.ListItem,
          parentId: educationId,
          order: 1,
          depth: 3,
        },
        {
          id: Guid.create(),
          content: '',
          comments: '',
          children: [],
          active: true,
          userId: this.resume.userId,
          nodeType: NodeType.ListItem,
          parentId: educationId,
          order: 2,
          depth: 3,
        },
        {
          id: Guid.create(),
          content: '',
          comments: '',
          children: [],
          active: true,
          userId: this.resume.userId,
          nodeType: NodeType.ListItem,
          parentId: educationId,
          order: 3,
          depth: 3,
        },
        {
          id: Guid.create(),
          content: '',
          comments: '',
          children: [],
          active: true,
          userId: this.resume.userId,
          nodeType: NodeType.ListItem,
          parentId: educationId,
          order: 4,
          depth: 3,
        },
        {
          id: Guid.create(),
          content: '',
          comments: '',
          children: [],
          active: true,
          userId: this.resume.userId,
          nodeType: NodeType.ListItem,
          parentId: educationId,
          order: 5,
          depth: 3,
        },
        {
          id: Guid.create(),
          content: '',
          comments: '',
          children: [],
          active: true,
          userId: this.resume.userId,
          nodeType: NodeType.ListItem,
          parentId: educationId,
          order: 6,
          depth: 3,
        },
      ],
      active: true,
      userId: this.resume.userId,
      nodeType: NodeType.Education,
      parentId: section.id,
      order: index,
      depth: 2,
    });

    this.form.addControl(
      `section${section.order}education${index}degree`,
      new FormControl(section.children[index].children[0].content, [
        Validators.required,
      ]),
    );

    this.form.controls[
      'section' + section.order + 'education' + index + 'degree'
    ].valueChanges.subscribe(res => {
      section.children[index].children[0].content = res;
    });

    this.form.addControl(
      `section${section.order}education${index}major`,
      new FormControl(section.children[index].children[1].content, [
        Validators.required,
      ]),
    );

    this.form.controls[
      'section' + section.order + 'education' + index + 'major'
    ].valueChanges.subscribe(res => {
      section.children[index].children[1].content = res;
    });

    this.form.addControl(
      `section${section.order}education${index}minor`,
      new FormControl(section.children[index].children[2].content, []),
    );

    this.form.controls[
      'section' + section.order + 'education' + index + 'minor'
    ].valueChanges.subscribe(res => {
      section.children[index].children[2].content = res;
    });

    this.form.addControl(
      `section${section.order}education${index}school`,
      new FormControl(section.children[index].children[3].content, [
        Validators.required,
      ]),
    );

    this.form.controls[
      'section' + section.order + 'education' + index + 'school'
    ].valueChanges.subscribe(res => {
      section.children[index].children[3].content = res;
    });

    this.form.addControl(
      `section${section.order}education${index}year`,
      new FormControl(section.children[index].children[4].content, [
        Validators.required,
        Validators.pattern('^[0-9]{4}$'),
      ]),
    );

    this.form.controls[
      'section' + section.order + 'education' + index + 'year'
    ].valueChanges.subscribe(res => {
      section.children[index].children[4].content = res;
    });

    this.form.addControl(
      `section${section.order}education${index}city`,
      new FormControl(section.children[index].children[5].content, [
        Validators.required,
      ]),
    );

    this.form.controls[
      'section' + section.order + 'education' + index + 'city'
    ].valueChanges.subscribe(res => {
      section.children[index].children[5].content = res;
    });

    this.form.addControl(
      `section${section.order}education${index}state`,
      new FormControl(section.children[index].children[6].content, [
        Validators.required,
        Validators.pattern('^[A-Z]{2}$'),
      ]),
    );

    this.form.controls[
      'section' + section.order + 'education' + index + 'state'
    ].valueChanges.subscribe(res => {
      section.children[index].children[6].content = res;
    });
  }

  removeEducation(section: ResumeTreeNode) {
    const nodeToDrop = section.children.pop();
    this.form.removeControl(
      `section${section.order}education${section.children.length}degree`,
    );
    this.form.removeControl(
      `section${section.order}education${section.children.length}major`,
    );
    this.form.removeControl(
      `section${section.order}education${section.children.length}minor`,
    );
    this.form.removeControl(
      `section${section.order}education${section.children.length}school`,
    );
    this.form.removeControl(
      `section${section.order}education${section.children.length}year`,
    );
    this.form.removeControl(
      `section${section.order}education${section.children.length}city`,
    );
    this.form.removeControl(
      `section${section.order}education${section.children.length}state`,
    );

    this.service.deleteNode(nodeToDrop).subscribe();
  }

  //endregion
  //region WorkExperience
  sectionAddWorkExperienceForm(section: ResumeTreeNode) {
    const index = section.children.length;
    const workExperienceId = Guid.create();
    section.children.push({
      id: workExperienceId,
      content: '',
      comments: '',
      children: [
        {
          id: Guid.create(),
          content: '',
          comments: '',
          children: [],
          active: true,
          userId: this.resume.userId,
          nodeType: NodeType.ListItem,
          parentId: workExperienceId,
          order: 0,
          depth: 3,
        },
        {
          id: Guid.create(),
          content: '',
          comments: '',
          children: [],
          active: true,
          userId: this.resume.userId,
          nodeType: NodeType.ListItem,
          parentId: workExperienceId,
          order: 1,
          depth: 3,
        },
        {
          id: Guid.create(),
          content: '',
          comments: '',
          children: [],
          active: true,
          userId: this.resume.userId,
          nodeType: NodeType.ListItem,
          parentId: workExperienceId,
          order: 2,
          depth: 3,
        },
        {
          id: Guid.create(),
          content: '',
          comments: '',
          children: [],
          active: true,
          userId: this.resume.userId,
          nodeType: NodeType.ListItem,
          parentId: workExperienceId,
          order: 3,
          depth: 3,
        },
        {
          id: Guid.create(),
          content: '',
          comments: '',
          children: [],
          active: true,
          userId: this.resume.userId,
          nodeType: NodeType.ListItem,
          parentId: workExperienceId,
          order: 4,
          depth: 3,
        },
        {
          id: Guid.create(),
          content: '',
          comments: '',
          children: [],
          active: true,
          userId: this.resume.userId,
          nodeType: NodeType.ListItem,
          parentId: workExperienceId,
          order: 5,
          depth: 3,
        },
      ],
      active: true,
      userId: this.resume.userId,
      nodeType: NodeType.WorkExperience,
      parentId: section.id,
      order: index,
      depth: 2,
    });

    this.form.addControl(
      `section${section.order}work${index}title`,
      new FormControl(section.children[index].children[0].content, [
        Validators.required,
      ]),
    );

    this.form.controls[
      'section' + section.order + 'work' + index + 'title'
    ].valueChanges.subscribe(res => {
      section.children[index].children[0].content = res;
    });

    this.form.addControl(
      `section${section.order}work${index}employer`,
      new FormControl(section.children[index].children[1].content, [
        Validators.required,
      ]),
    );

    this.form.controls[
      'section' + section.order + 'work' + index + 'employer'
    ].valueChanges.subscribe(res => {
      section.children[index].children[1].content = res;
    });

    this.form.addControl(
      `section${section.order}work${index}city`,
      new FormControl(section.children[index].children[2].content, [
        Validators.required,
      ]),
    );

    this.form.controls[
      'section' + section.order + 'work' + index + 'city'
    ].valueChanges.subscribe(res => {
      section.children[index].children[2].content = res;
    });

    this.form.addControl(
      `section${section.order}work${index}state`,
      new FormControl(section.children[index].children[3].content, [
        Validators.required,
        Validators.pattern('^[A-Z]{2}$'),
      ]),
    );

    this.form.controls[
      'section' + section.order + 'work' + index + 'state'
    ].valueChanges.subscribe(res => {
      section.children[index].children[3].content = res;
    });

    const startPattern: RegExp = /(0[1-9]|1[1,2])([\/\-])(19|20)\d{2}/;

    this.form.addControl(
      `section${section.order}work${index}start`,
      new FormControl(section.children[index].children[4].content, [
        Validators.required,
        Validators.pattern(startPattern),
      ]),
    );

    this.form.controls[
      'section' + section.order + 'work' + index + 'start'
    ].valueChanges.subscribe(res => {
      section.children[index].children[4].content = res;
    });

    const endPattern: RegExp = /((0[1-9]|1[1,2])([\/\-])(19|20)\d{2}|Present)/;

    this.form.addControl(
      `section${section.order}work${index}end`,
      new FormControl(section.children[index].children[5].content, [
        Validators.required,
        Validators.pattern(endPattern),
      ]),
    );

    this.form.controls[
      'section' + section.order + 'work' + index + 'end'
    ].valueChanges.subscribe(res => {
      section.children[index].children[5].content = res;
    });

    this.form.addControl(
      `section${section.order}work${index}pagebreak`,
      new FormControl(false),
    );

    this.form.controls[
      `section${section.order}work${index}pagebreak`
    ].valueChanges.subscribe(res => {
      section.children[index].comments = res ? 'page-break' : '';
    });
  }

  removeWorkExperience(section: ResumeTreeNode) {
    const nodeToDrop = section.children.pop();

    this.form.removeControl(
      `section${section.order}work${section.children.length}title`,
    );
    this.form.removeControl(
      `section${section.order}work${section.children.length}employer`,
    );
    this.form.removeControl(
      `section${section.order}work${section.children.length}city`,
    );
    this.form.removeControl(
      `section${section.order}work${section.children.length}state`,
    );
    this.form.removeControl(
      `section${section.order}work${section.children.length}start`,
    );
    this.form.removeControl(
      `section${section.order}work${section.children.length}end`,
    );
    this.form.removeControl(
      `section${section.order}work${section.children.length}pagebreak`,
    );

    this.service.deleteNode(nodeToDrop).subscribe();
  }

  createResponsibilities(sectionOrder: number, job: ResumeTreeNode) {
    const id = Guid.create();
    job.children.push({
      children: [
        {
          children: [],
          active: true,
          userId: this.resume.userId,
          nodeType: NodeType.ListItem,
          parentId: id,
          order: 0,
          depth: 4,
          content: '',
          id: Guid.create(),
          comments: '',
        },
      ],
      active: true,
      userId: this.resume.userId,
      nodeType: NodeType.Responsibilities,
      parentId: job.id,
      order: 6,
      depth: 3,
      content: '',
      id: id,
      comments: '',
    });

    console.log(
      `creating section${sectionOrder}work${job.order}responsibility0`,
    );

    this.form.addControl(
      `section${sectionOrder}work${job.order}responsibility0`,
      new FormControl(job.children[6].children[0].content, [
        Validators.required,
      ]),
    );

    this.form.controls[
      'section' + sectionOrder + 'work' + job.order + 'responsibility0'
    ].valueChanges.subscribe(res => {
      job.children[6].children[0].content = res;
    });
  }

  addResponsibilities(sectionOrder: number, job: ResumeTreeNode) {
    console.log(job);
    const id = job.children[6].id;
    const index = job.children[6].children.length;
    job.children[6].children.push({
      children: [],
      active: true,
      userId: this.resume.userId,
      nodeType: NodeType.ListItem,
      parentId: id,
      order: index,
      depth: 4,
      content: '',
      id: Guid.create(),
      comments: '',
    });

    console.log(
      `creating section${sectionOrder}work${job.order}responsibility${index}`,
    );

    this.form.addControl(
      `section${sectionOrder}work${job.order}responsibility${index}`,
      new FormControl(job.children[6].children[index].content, [
        Validators.required,
      ]),
    );

    this.form.controls[
      'section' + sectionOrder + 'work' + job.order + 'responsibility' + index
    ].valueChanges.subscribe(res => {
      job.children[6].children[index].content = res;
    });
  }

  removeResponsibilities(sectionOrder: number, job: ResumeTreeNode) {
    const nodeToDrop = job.children[6].children.pop();
    this.form.removeControl(
      `section${sectionOrder}work${job.order}responsibility${job.children.length}`,
    );

    this.service.deleteNode(nodeToDrop).subscribe();

    if (job.children[6].children.length == 0) {
      this.service.deleteNode(job.children[6]).subscribe();
      job.children.pop();
    }
  }

  //endregion
  //endregion
  //endregion
}
