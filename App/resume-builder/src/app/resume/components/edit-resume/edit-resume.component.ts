import { Component, OnInit, QueryList, ViewChildren } from '@angular/core';
import {
  newResumeTreeNode,
  NodeType,
  ResumeTreeNode,
  SectionDisplayType,
} from '../../../models/Resume';
import { Router } from '@angular/router';
import { ResumeService } from '../../services/resume.service';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Guid } from 'guid-typescript';
import {
  BorderStyle,
  ButtonComponent,
  ButtonStyle,
} from '../../../common/button/button.component';
import { ToastrService } from 'ngx-toastr';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import { InputComponent } from '../../../common/input/input.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ResumeSectionComponent } from './components/resume-section/resume-section.component';
import { PersonalInfoComponent } from './components/personal-info/personal-info.component';
import { concat, Observable, toArray } from 'rxjs';
import { MatExpansionModule } from '@angular/material/expansion';

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
    MatExpansionModule,
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
        this.queueSave(this.resume);
      });

      this.loading = false;
    });
  }

  save(): void {
    const apiCalls: Observable<any>[] = [];
    this.saves = this.saves
      .filter(node => !this.deletes.includes(node.id))
      .sort(this.sortSaves());
    apiCalls.push(...this.deletes.map(id => this.service.deleteNode(id)));
    if (this.saves.length > 0)
      apiCalls.push(this.service.updateResume(this.saves));

    if (apiCalls.length === 0) return;

    concat(...apiCalls)
      .pipe(toArray())
      .subscribe({
        next: () => {
          this.saves = [];
          this.deletes = [];
          this.toaster.success('Resume saved successfully', 'Saved');
          this.refreshPreview();
        },
        error: error => {
          this.toaster.error('Error saving resume: ' + error, 'Error');
        },
      });
  }

  sortSaves() {
    return (a: ResumeTreeNode, b: ResumeTreeNode) => {
      if (a.depth === b.depth) {
        return a.order - b.order;
      }
      return a.depth - b.depth;
    };
  }

  refreshPreview(): void {
    this.service
      .getPreview(this.resume)
      .subscribe(res => (this.pdfSource = res));
  }

  //region Section

  addSection(): void {
    const index = this.resume.children.length;

    this.resume.children.push(
      newResumeTreeNode(NodeType.Section, index, this.resume),
    );
  }

  removeSection($index: number) {
    this.queueDelete(this.resume.children[$index].id);
    this.resume.children = this.resume.children.filter((_, i) => i !== $index);
    this.reorderChildren();
  }

  reorderChildren() {
    this.resume.children.forEach((child, index) => {
      child.order = index;
      this.queueSave(child);
    });
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
  }

  queueDelete(id: Guid) {
    this.deletes.push(id);
  }

  protected readonly ButtonStyle = ButtonStyle;
  protected readonly BorderStyle = BorderStyle;
  protected readonly SectionDisplayType = SectionDisplayType;
}
