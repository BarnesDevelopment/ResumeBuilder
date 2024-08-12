import { Component, Input, OnInit } from '@angular/core';
import {
  newResumeTreeNode,
  NodeType,
  ResumeTreeNode,
} from '../../../../../models/Resume';
import { UpsertSignal } from '../upsert-signal/upsert-signal';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { InputComponent } from '../../../../../common/input/input.component';
import { SectionListComponent } from '../section-list/section-list.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-section-work-experience',
  standalone: true,
  imports: [
    InputComponent,
    ReactiveFormsModule,
    SectionListComponent,
    MatProgressSpinnerModule,
  ],
  templateUrl: './section-work-experience.component.html',
  styleUrl: './section-work-experience.component.scss',
})
export class SectionWorkExperienceComponent
  extends UpsertSignal
  implements OnInit
{
  @Input() node: ResumeTreeNode;
  form: FormGroup;
  loading: boolean = true;

  ngOnInit(): void {
    if (!this.node.children.length) {
      this.CreateChildren();
      this.node.children.forEach(child => {
        this.onSave.emit(child);
      });
    }
    this.form = new FormGroup({
      title: new FormControl(this.node.children[0].content),
      employer: new FormControl(this.node.children[1].content),
      city: new FormControl(this.node.children[2].content),
      state: new FormControl(this.node.children[3].content),
      startDate: new FormControl(this.node.children[4].content),
      endDate: new FormControl(this.node.children[5].content),
    });
    this.form.controls['title'].valueChanges.subscribe(value => {
      this.node.children[0].content = value;
      this.onSave.emit(this.node.children[0]);
    });
    this.form.controls['employer'].valueChanges.subscribe(value => {
      this.node.children[1].content = value;
      this.onSave.emit(this.node.children[1]);
    });
    this.form.controls['city'].valueChanges.subscribe(value => {
      this.node.children[2].content = value;
      this.onSave.emit(this.node.children[2]);
    });
    this.form.controls['state'].valueChanges.subscribe(value => {
      this.node.children[3].content = value;
      this.onSave.emit(this.node.children[3]);
    });
    this.form.controls['startDate'].valueChanges.subscribe(value => {
      this.node.children[4].content = value;
      this.onSave.emit(this.node.children[4]);
    });
    this.form.controls['endDate'].valueChanges.subscribe(value => {
      this.node.children[5].content = value;
      this.onSave.emit(this.node.children[5]);
    });
    this.loading = false;
  }

  CreateChildren() {
    this.node.children = [
      newResumeTreeNode(NodeType.ListItem, 0, this.node),
      newResumeTreeNode(NodeType.ListItem, 1, this.node),
      newResumeTreeNode(NodeType.ListItem, 2, this.node),
      newResumeTreeNode(NodeType.ListItem, 3, this.node),
      newResumeTreeNode(NodeType.ListItem, 4, this.node),
      newResumeTreeNode(NodeType.ListItem, 5, this.node),
      newResumeTreeNode(NodeType.List, 6, this.node),
    ];
  }
}
