import { Component, OnInit, QueryList, ViewChildren } from '@angular/core';
import {
  ResumeTreeNode,
  SectionDisplayType,
  NodeType,
  newResumeTreeNode,
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
import { PersonalInfoComponent } from './components/personal-info/personal-info.component';
import { combineLatest, Observable } from 'rxjs';

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
    PersonalInfoComponent,
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

  saves: ResumeTreeNode[] = [];
  deletes: Guid[] = [];

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
      });

      this.form.valueChanges.subscribe(() => {
        this.resume.content = this.form.get('title').value;
        this.resume.comments = this.form.get('comments').value;
      });

      this.loading = false;
    });
  }

  save(): void {
    const apiCalls: Observable<any>[] = [];
    this.saves = this.saves.filter(node => !this.deletes.includes(node.id));
    this.saves.forEach(node => apiCalls.push(this.service.updateResume(node)));
    this.deletes.forEach(id => apiCalls.push(this.service.deleteNode(id)));
    // console.log({ saves: this.saves, deletes: this.deletes });
    if (apiCalls.length === 0) return;
    combineLatest(apiCalls).subscribe({
      next: () => {
        this.saves = [];
        this.deletes = [];
        this.toaster.success('Resume saved successfully', 'Saved');
        this.refreshPreview();
        console.log('Resume saved successfully');
      },
      error: error => {
        this.toaster.error('Error saving resume: ' + error, 'Error');
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

    this.resume.children.push(
      newResumeTreeNode(NodeType.Section, index, this.resume),
    );
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

  queueSave(node: ResumeTreeNode) {
    const oldSave = this.saves.find(n => n.id === node.id);
    if (oldSave) this.saves.splice(this.saves.indexOf(oldSave), 1);
    this.saves.push(node);
    console.log({ saves: this.saves });
  }

  queueDelete(id: Guid) {
    this.deletes.push(id);
    console.log({ deletes: this.deletes });
  }

  protected readonly ButtonStyle = ButtonStyle;
  protected readonly BorderStyle = BorderStyle;
  protected readonly SectionDisplayType = SectionDisplayType;
}
