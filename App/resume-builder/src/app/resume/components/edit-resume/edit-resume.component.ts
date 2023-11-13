import { Component, OnInit } from '@angular/core';
import {
  ResumeTreeNode,
  SectionDisplayType,
  NodeType,
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
    this.service.getResume(this.router.url.split('/')[2]).subscribe(res => {
      this.resume = res;
      console.log(this.resume);
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

      this.resume.children.forEach(node => {
        this.createSectionFormControls(node);
      });
      this.loading = false;
    });
  }

  title(): ResumeTreeNode {
    return this.resume.children.find(node => node.nodeType === NodeType.Title);
  }

  save(): void {
    this.service.updateResume(this.resume).subscribe();
  }

  //region Section
  sections(): ResumeTreeNode[] {
    return this.resume.children.filter(
      node => node.nodeType === NodeType.Section,
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
      res => {
        this.resume.children[node.order].content = res;
      },
    );

    node.children.forEach(child => {
      switch (child.nodeType) {
        case NodeType.List:
          this.initializeList(node.order, child);
          break;
        case NodeType.Paragraph:
          this.initializeParagraph(node.order, child);
          break;
        case NodeType.Education:
          this.initializeEducation(node.order, child);
          break;
        case NodeType.WorkExperience:
          this.initializeWorkExperience(node.order, child);
          break;
        default:
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
      nodeType: NodeType.Section,
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
          nodeType: NodeType.Separator,
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

    this.form.controls[`section${index}title`].valueChanges.subscribe(res => {
      this.resume.children[index].content = res;
      this.resume.children[index].children[0].content = res;
    });
  }
  //endregion

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

  initializeList(sectionOrder: number, list: ResumeTreeNode) {
    this.form.controls[`section${sectionOrder}type`].setValue(
      SectionDisplayType.List,
    );
    list.children.forEach(node => {
      this.form.addControl(
        `section${sectionOrder}listItem${node.order}`,
        new FormControl(node.content, [Validators.required]),
      );

      this.form.controls[
        `section${sectionOrder}listItem${node.order}`
      ].valueChanges.subscribe(res => {
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
  initializeParagraph(sectionOrder: number, paragraph: ResumeTreeNode) {
    this.form.controls[`section${sectionOrder}type`].setValue(
      SectionDisplayType.Paragraph,
    );

    this.form.addControl(
      `section${sectionOrder}paragraph`,
      new FormControl(paragraph.content, [Validators.required]),
    );

    this.form.controls[
      `section${sectionOrder}paragraph`
    ].valueChanges.subscribe(res => {
      paragraph.content = res;
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

  initializeEducation(sectionOrder: number, education: ResumeTreeNode) {
    this.form.controls[`section${sectionOrder}type`].setValue(
      SectionDisplayType.Education,
    );

    this.form.addControl(
      `section${sectionOrder}education${education.order}degree`,
      new FormControl(
        this.resume.children[sectionOrder].children[
          education.order
        ].children[0].content,
        [Validators.required],
      ),
    );

    this.form.controls[
      'section' + sectionOrder + 'education' + education.order + 'degree'
    ].valueChanges.subscribe(res => {
      this.resume.children[sectionOrder].children[
        education.order
      ].children[0].content = res;
    });

    this.form.addControl(
      `section${sectionOrder}education${education.order}major`,
      new FormControl(
        this.resume.children[sectionOrder].children[
          education.order
        ].children[1].content,
        [Validators.required],
      ),
    );

    this.form.controls[
      'section' + sectionOrder + 'education' + education.order + 'major'
    ].valueChanges.subscribe(res => {
      this.resume.children[sectionOrder].children[
        education.order
      ].children[1].content = res;
    });

    this.form.addControl(
      `section${sectionOrder}education${education.order}minor`,
      new FormControl(
        this.resume.children[sectionOrder].children[
          education.order
        ].children[2].content,
        [],
      ),
    );

    this.form.controls[
      'section' + sectionOrder + 'education' + education.order + 'minor'
    ].valueChanges.subscribe(res => {
      this.resume.children[sectionOrder].children[
        education.order
      ].children[2].content = res;
    });

    this.form.addControl(
      `section${sectionOrder}education${education.order}school`,
      new FormControl(
        this.resume.children[sectionOrder].children[
          education.order
        ].children[3].content,
        [Validators.required],
      ),
    );

    this.form.controls[
      'section' + sectionOrder + 'education' + education.order + 'school'
    ].valueChanges.subscribe(res => {
      this.resume.children[sectionOrder].children[
        education.order
      ].children[3].content = res;
    });

    this.form.addControl(
      `section${sectionOrder}education${education.order}year`,
      new FormControl(
        this.resume.children[sectionOrder].children[
          education.order
        ].children[4].content,
        [Validators.required, Validators.pattern('^[0-9]{4}$')],
      ),
    );

    this.form.controls[
      'section' + sectionOrder + 'education' + education.order + 'year'
    ].valueChanges.subscribe(res => {
      this.resume.children[sectionOrder].children[
        education.order
      ].children[4].content = res;
    });

    this.form.addControl(
      `section${sectionOrder}education${education.order}city`,
      new FormControl(
        this.resume.children[sectionOrder].children[
          education.order
        ].children[5].content,
        [Validators.required],
      ),
    );

    this.form.controls[
      'section' + sectionOrder + 'education' + education.order + 'city'
    ].valueChanges.subscribe(res => {
      this.resume.children[sectionOrder].children[
        education.order
      ].children[5].content = res;
    });

    this.form.addControl(
      `section${sectionOrder}education${education.order}state`,
      new FormControl(
        this.resume.children[sectionOrder].children[
          education.order
        ].children[6].content,
        [Validators.required, Validators.pattern('^[A-Z]{2}$')],
      ),
    );

    this.form.controls[
      'section' + sectionOrder + 'education' + education.order + 'state'
    ].valueChanges.subscribe(res => {
      this.resume.children[sectionOrder].children[
        education.order
      ].children[6].content = res;
    });
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

    this.service.deleteNode(nodeToDrop).subscribe();
  }
  initializeWorkExperience(
    sectionOrder: number,
    workExperience: ResumeTreeNode,
  ) {
    this.form.controls[`section${sectionOrder}type`].setValue(
      SectionDisplayType.WorkExperience,
    );

    this.form.addControl(
      `section${sectionOrder}work${workExperience.order}title`,
      new FormControl(
        this.resume.children[sectionOrder].children[
          workExperience.order
        ].children[0].content,
        [Validators.required],
      ),
    );

    this.form.controls[
      'section' + sectionOrder + 'work' + workExperience.order + 'title'
    ].valueChanges.subscribe(res => {
      this.resume.children[sectionOrder].children[
        workExperience.order
      ].children[0].content = res;
    });

    this.form.addControl(
      `section${sectionOrder}work${workExperience.order}employer`,
      new FormControl(
        this.resume.children[sectionOrder].children[
          workExperience.order
        ].children[1].content,
        [Validators.required],
      ),
    );

    this.form.controls[
      'section' + sectionOrder + 'work' + workExperience.order + 'employer'
    ].valueChanges.subscribe(res => {
      this.resume.children[sectionOrder].children[
        workExperience.order
      ].children[1].content = res;
    });

    this.form.addControl(
      `section${sectionOrder}work${workExperience.order}city`,
      new FormControl(
        this.resume.children[sectionOrder].children[
          workExperience.order
        ].children[2].content,
        [Validators.required],
      ),
    );

    this.form.controls[
      'section' + sectionOrder + 'work' + workExperience.order + 'city'
    ].valueChanges.subscribe(res => {
      this.resume.children[sectionOrder].children[
        workExperience.order
      ].children[2].content = res;
    });

    this.form.addControl(
      `section${sectionOrder}work${workExperience.order}state`,
      new FormControl(
        this.resume.children[sectionOrder].children[
          workExperience.order
        ].children[3].content,
        [Validators.required, Validators.pattern('^[A-Z]{2}$')],
      ),
    );

    this.form.controls[
      'section' + sectionOrder + 'work' + workExperience.order + 'state'
    ].valueChanges.subscribe(res => {
      this.resume.children[sectionOrder].children[
        workExperience.order
      ].children[3].content = res;
    });

    const startPattern: RegExp = /(0[1-9]|1[1,2])([\/\-])(19|20)\d{2}/;

    this.form.addControl(
      `section${sectionOrder}work${workExperience.order}start`,
      new FormControl(
        this.resume.children[sectionOrder].children[
          workExperience.order
        ].children[4].content,
        [Validators.required, Validators.pattern(startPattern)],
      ),
    );

    this.form.controls[
      'section' + sectionOrder + 'work' + workExperience.order + 'start'
    ].valueChanges.subscribe(res => {
      this.resume.children[sectionOrder].children[
        workExperience.order
      ].children[4].content = res;
    });

    const endPattern: RegExp = /((0[1-9]|1[1,2])([\/\-])(19|20)\d{2}|Present)/;

    this.form.addControl(
      `section${sectionOrder}work${workExperience.order}end`,
      new FormControl(
        this.resume.children[sectionOrder].children[
          workExperience.order
        ].children[5].content,
        [Validators.required, Validators.pattern(endPattern)],
      ),
    );

    this.form.controls[
      'section' + sectionOrder + 'work' + workExperience.order + 'end'
    ].valueChanges.subscribe(res => {
      this.resume.children[sectionOrder].children[
        workExperience.order
      ].children[5].content = res;
    });
  }

  sectionAddResponsibilities(job: ResumeTreeNode) {}
  //endregion
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
      nodeType: NodeType.ListItem,
      parentId: this.title().id,
      order: index,
      depth: 2,
    });

    this.form.addControl(
      'website',
      new FormControl(this.title().children[index].content),
    );

    this.form.controls['website'].valueChanges.subscribe(res => {
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
