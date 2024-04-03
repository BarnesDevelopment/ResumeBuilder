import { Component, OnInit } from '@angular/core';
import {
  ResumeTreeNode,
  SectionDisplayType,
  NodeType,
} from '../../../models/Resume';
import { Router } from '@angular/router';
import { ResumeService } from '../../services/resume.service';
import {
  FormControl,
  FormGroup,
  Validators,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { Guid } from 'guid-typescript';
import {
  BorderStyle,
  ButtonStyle,
  ButtonComponent,
} from '../../../common/button/button.component';
import { ToastrService } from 'ngx-toastr';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import { InputComponent } from '../../../common/input/input.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { SectionComponent } from './components/section/section.component';

@Component({
  selector: 'app-edit-resume',
  templateUrl: './edit-resume.component.html',
  styleUrls: ['./edit-resume.component.scss'],
  standalone: true,
  imports: [
    MatProgressSpinnerModule,
    FormsModule,
    ReactiveFormsModule,
    InputComponent,
    ButtonComponent,
    NgxExtendedPdfViewerModule,
    SectionComponent,
  ],
})
export class EditResumeComponent implements OnInit {
  resume: ResumeTreeNode;
  loading: boolean = true;
  form: FormGroup;
  phonePattern =
    '^(\\+\\d{1,2}\\s?)?\\(?\\d{3}\\)?[\\s.-]?\\d{3}[\\s.-]?\\d{4}$';
  platform: string = '';
  pdfSource: Blob;

  constructor(
    private router: Router,
    private service: ResumeService,
    private toaster: ToastrService,
  ) {}

  ngOnInit(): void {
    this.platform = navigator.platform;
    this.service.getResume(this.router.url.split('/')[2]).subscribe(res => {
      this.resume = res;

      this.refreshPreview();

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
    this.service.updateResume(this.resume).subscribe(
      () => {
        this.toaster.success('Resume saved successfully', 'Saved');
        this.refreshPreview();
      },
      err => {
        this.toaster.error('Error saving resume: ' + err.message, 'Error');
      },
    );
  }

  refreshPreview(): void {
    this.service
      .getPreview(this.resume)
      .subscribe(res => (this.pdfSource = res));
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

    this.form.addControl(
      `section${node.order}pagebreak`,
      new FormControl(
        this.resume.children[node.order].comments === 'page-break',
      ),
    );

    this.form.controls[`section${node.order}pagebreak`].valueChanges.subscribe(
      res => {
        this.resume.children[node.order].comments = res ? 'page-break' : '';
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

  initializeWorkExperience(
    sectionOrder: number,
    workExperience: ResumeTreeNode,
  ) {
    this.form.controls[`section${sectionOrder}type`].setValue(
      SectionDisplayType.WorkExperience,
    );

    this.form.addControl(
      `section${sectionOrder}work${workExperience.order}pagebreak`,
      new FormControl(workExperience.comments === 'page-break'),
    );

    this.form.controls[
      `section${sectionOrder}work${workExperience.order}pagebreak`
    ].valueChanges.subscribe(res => {
      workExperience.comments = res ? 'page-break' : '';
    });

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

    if (workExperience.children.length > 6) {
      this.initializeResponsibilities(sectionOrder, workExperience);
    }
  }

  initializeResponsibilities(sectionOrder: number, job: ResumeTreeNode) {
    job.children[6].children.forEach(node => {
      this.form.addControl(
        `section${sectionOrder}work${job.order}responsibility${node.order}`,
        new FormControl(node.content, [Validators.required]),
      );
      this.form.controls[
        'section' +
          sectionOrder +
          'work' +
          job.order +
          'responsibility' +
          node.order
      ].valueChanges.subscribe(res => {
        job.children[node.order].children[0].content = res;
      });
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

    this.form.addControl(`section${index}pagebreak`, new FormControl(false));

    this.form.controls[`section${index}pagebreak`].valueChanges.subscribe(
      res => {
        this.resume.children[index].comments = res ? 'page-break' : '';
      },
    );

    this.form.controls[`section${index}title`].valueChanges.subscribe(res => {
      this.resume.children[index].content = res;
      this.resume.children[index].children[0].content = res;
    });
  }

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

  onKeyDown($event: KeyboardEvent): void {
    // Detect platform
    if (navigator.platform.match('Mac')) {
      this.handleMacKeyEvents($event);
    } else {
      this.handleWindowsKeyEvents($event);
    }
  }

  handleMacKeyEvents($event: KeyboardEvent) {
    // MetaKey documentation
    // https://developer.mozilla.org/en-US/docs/Web/API/KeyboardEvent/metaKey
    let charCode = String.fromCharCode($event.which).toLowerCase();
    if ($event.metaKey && charCode === 's') {
      this.save();
      $event.preventDefault();
    }
  }

  handleWindowsKeyEvents($event: KeyboardEvent) {
    let charCode = String.fromCharCode($event.which).toLowerCase();
    if ($event.ctrlKey && charCode === 's') {
      this.save();
      $event.preventDefault();
    }
  }

  protected readonly ButtonStyle = ButtonStyle;
  protected readonly BorderStyle = BorderStyle;
  protected readonly SectionDisplayType = SectionDisplayType;
}
