import { Component, OnInit } from '@angular/core';
import {
  ResumeTreeNode,
  SectionDisplayType,
  SectionType,
} from '../../../models/Resume';
import { Router } from '@angular/router';
import { ResumeService } from '../../services/resume.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Guid } from 'guid-typescript';
import {
  BorderStyle,
  ButtonStyle,
} from '../../../common/button/button.component';

@Component({
  selector: 'app-edit-resume',
  templateUrl: './edit-resume.component.html',
  styleUrls: ['./edit-resume.component.scss'],
})
export class EditResumeComponent implements OnInit {
  resume: ResumeTreeNode;
  loading: boolean = true;
  form: FormGroup;
  phonePattern =
    '^(\\+\\d{1,2}\\s?)?\\(?\\d{3}\\)?[\\s.-]?\\d{3}[\\s.-]?\\d{4}$';

  constructor(
    private router: Router,
    private service: ResumeService,
  ) {}

  ngOnInit(): void {
    this.service.getResume(this.router.url.split('/')[2]).subscribe((res) => {
      this.resume = res;
      this.form = new FormGroup({
        title: new FormControl(this.resume.content, [Validators.required]),
        comments: new FormControl(this.resume.comments),
        name: new FormControl(this.title().content, [Validators.required]),
        email: new FormControl(this.title().children[0].content, [
          Validators.required,
          Validators.email,
        ]),
        phone: new FormControl(this.title().children[1].content, [
          Validators.required,
          Validators.pattern(this.phonePattern),
        ]),
      });
      if (this.title().children.length > 2) {
        this.form.addControl(
          'website',
          new FormControl(this.title().children[2].content),
        );
      }

      this.resume.children.forEach((node) => {
        this.createSectionFormControls(node);
      });
      this.loading = false;
    });
  }

  title(): ResumeTreeNode {
    return this.resume.children.find(
      (node) => node.sectionType === SectionType.Title,
    );
  }

  save(): void {
    this.service.updateResume(this.resume).subscribe();
  }

  //region Section
  sections(): ResumeTreeNode[] {
    return this.resume.children.filter(
      (node) => node.sectionType === SectionType.Section,
    );
  }

  //region Modify
  createSectionFormControls(node: ResumeTreeNode): void {
    this.form.addControl(
      `section${node.order}title`,
      new FormControl(this.resume.children[node.order].content, [
        Validators.required,
      ]),
    );

    this.form.addControl(
      `section${node.order}type`,
      new FormControl('', [Validators.required]),
    );

    this.form.controls[`section${node.order}title`].valueChanges.subscribe(
      (res) => {
        this.resume.children[node.order].content = res;
      },
    );

    node.children.forEach((child) => {
      switch (child.sectionType) {
        default:
          break;
        case SectionType.List:
          this.initializeList(node.order, child);
          break;
      }
    });
  }

  addSection(): void {
    const index = this.resume.children.length;
    const sectionId = Guid.create();

    this.resume.children.push({
      id: sectionId,
      content: 'Section Title',
      comments: '',
      active: true,
      userId: this.resume.userId,
      sectionType: SectionType.Section,
      parentId: this.resume.id,
      order: index,
      depth: 1,
      children: [
        {
          id: Guid.create(),
          content: 'Section Title',
          comments: '',
          active: true,
          userId: this.resume.userId,
          sectionType: SectionType.Separator,
          parentId: sectionId,
          order: 0,
          depth: 2,
          children: [],
        },
      ],
    });

    this.form.addControl(
      `section${index}title`,
      new FormControl(this.resume.children[index].content, [
        Validators.required,
      ]),
    );

    this.form.addControl(
      `section${index}type`,
      new FormControl('', [Validators.required]),
    );

    this.form.controls[`section${index}title`].valueChanges.subscribe((res) => {
      this.resume.children[index].content = res;
      this.resume.children[index].children[0].content = res;
    });
  }
  //endregion

  //region SectionType
  sectionTypeChanged(section: ResumeTreeNode) {
    section.children.forEach((node) => {
      this.service.deleteNode(node).subscribe();
      this.form.removeControl(`section${section.order}listItem${node.order}`);
    });
    section.children = [];

    switch (this.form.controls[`section${section.order}type`].value) {
      default:
        break;
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
          sectionType: SectionType.ListItem,
          parentId: listId,
          order: 0,
          depth: 3,
        },
      ],
      active: true,
      userId: this.resume.userId,
      sectionType: SectionType.List,
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
    ].valueChanges.subscribe((res) => {
      section.children[index].children[0].content = res;
    });
  }

  initializeList(sectionOrder: number, list: ResumeTreeNode) {
    this.form.controls[`section${sectionOrder}type`].setValue(
      SectionDisplayType.List,
    );
    list.children.forEach((node) => {
      this.form.addControl(
        `section${sectionOrder}listItem${node.order}`,
        new FormControl(node.content, [Validators.required]),
      );

      this.form.controls[
        `section${sectionOrder}listItem${node.order}`
      ].valueChanges.subscribe((res) => {
        node.content = res;
      });
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
      sectionType: SectionType.ListItem,
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
    ].valueChanges.subscribe((res) => {
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
  sectionAddParagraphForm(section: ResumeTreeNode) {
    const index = section.children.length;
    section.children.push({
      id: Guid.create(),
      content: '',
      comments: '',
      children: [],
      active: true,
      userId: this.resume.userId,
      sectionType: SectionType.ListItem,
      parentId: section.id,
      order: index,
      depth: 2,
    });

    this.form.addControl(
      `section${section.order}paragraph${index}`,
      new FormControl(section.children[index].content, [Validators.required]),
    );

    this.form.controls[
      `section${section.order}paragraph${index}`
    ].valueChanges.subscribe((res) => {
      section.children[index].content = res;
    });
  }
  sectionAddEducationForm(section: ResumeTreeNode) {}
  sectionAddWorkExperienceForm(section: ResumeTreeNode) {}
  //endregion
  //endregion
  //endregion

  //region Website
  addWebsite(): void {
    const index = this.title().children.length;
    this.title().children.push({
      id: Guid.create(),
      content: '',
      comments: '',
      children: [],
      active: true,
      userId: this.resume.userId,
      sectionType: SectionType.ListItem,
      parentId: this.title().id,
      order: index,
      depth: 2,
    });

    this.form.addControl(
      'website',
      new FormControl(this.title().children[index].content),
    );

    this.form.controls['website'].valueChanges.subscribe((res) => {
      this.title().children[index].content = res;
    });
  }

  removeWebsite(): void {
    const nodeToDrop = this.title().children.pop();
    this.form.removeControl('website');

    this.service.deleteNode(nodeToDrop).subscribe();
  }
  //endregion

  protected readonly ButtonStyle = ButtonStyle;
  protected readonly BorderStyle = BorderStyle;
  protected readonly SectionDisplayType = SectionDisplayType;
}
