import {
  AfterViewInit,
  Component,
  OnInit,
  QueryList,
  ViewChildren,
} from '@angular/core';
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
import { ResumeSectionComponent } from './components/resume-section/resume-section.component';

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
    ResumeSectionComponent,
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

  @ViewChildren(ResumeSectionComponent)
  sectionsElements: QueryList<ResumeSectionComponent>;

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

      this.loading = false;
    });
  }

  title(): ResumeTreeNode {
    return this.resume.children.find(node => node.nodeType === NodeType.Title);
  }

  save(): void {
    this.service.updateResume(this.resume).subscribe({
      next: () => {
        this.toaster.success('Resume saved successfully', 'Saved');
        this.refreshPreview();
      },
      error: err => {
        this.toaster.error('Error saving resume: ' + err.message, 'Error');
      },
    });
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
          content: '',
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
