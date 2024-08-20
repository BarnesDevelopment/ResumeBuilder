import { Component, Input, OnChanges } from '@angular/core';
import {
  newResumeTreeNode,
  NodeType,
  ResumeTreeNode,
} from '../../../../../models/Resume';
import { UpsertSignal } from '../upsert-signal/upsert-signal';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { InputComponent } from '../../../../../common/input/input.component';

@Component({
  selector: 'app-section-education',
  standalone: true,
  imports: [InputComponent, ReactiveFormsModule],
  templateUrl: './section-education.component.html',
  styleUrl: './section-education.component.scss',
})
export class SectionEducationComponent
  extends UpsertSignal
  implements OnChanges
{
  @Input() node: ResumeTreeNode;

  form: FormGroup;

  ngOnChanges() {
    if (!this.node.children.length) {
      this.CreateChildren();
      this.node.children.forEach(child => {
        this.onSave.emit(child);
      });
    }
    this.form = new FormGroup({
      degree: new FormControl(this.node.children[0].content),
      major: new FormControl(this.node.children[1].content),
      minor: new FormControl(this.node.children[2].content),
      school: new FormControl(this.node.children[3].content),
      city: new FormControl(this.node.children[4].content),
      state: new FormControl(this.node.children[5].content),
      graduationYear: new FormControl(this.node.children[6].content),
    });
    this.form.controls['degree'].valueChanges.subscribe(value => {
      this.node.children[0].content = value;
      this.onSave.emit(this.node.children[0]);
    });
    this.form.controls['major'].valueChanges.subscribe(value => {
      this.node.children[1].content = value;
      this.onSave.emit(this.node.children[1]);
    });
    this.form.controls['minor'].valueChanges.subscribe(value => {
      this.node.children[2].content = value;
      this.onSave.emit(this.node.children[2]);
    });
    this.form.controls['school'].valueChanges.subscribe(value => {
      this.node.children[3].content = value;
      this.onSave.emit(this.node.children[3]);
    });
    this.form.controls['city'].valueChanges.subscribe(value => {
      this.node.children[4].content = value;
      this.onSave.emit(this.node.children[4]);
    });
    this.form.controls['state'].valueChanges.subscribe(value => {
      this.node.children[5].content = value;
      this.onSave.emit(this.node.children[5]);
    });
    this.form.controls['graduationYear'].valueChanges.subscribe(value => {
      this.node.children[6].content = value;
      this.onSave.emit(this.node.children[6]);
    });
  }

  CreateChildren() {
    this.node.children = [
      newResumeTreeNode(NodeType.ListItem, 0, this.node),
      newResumeTreeNode(NodeType.ListItem, 1, this.node),
      newResumeTreeNode(NodeType.ListItem, 2, this.node),
      newResumeTreeNode(NodeType.ListItem, 3, this.node),
      newResumeTreeNode(NodeType.ListItem, 4, this.node),
      newResumeTreeNode(NodeType.ListItem, 5, this.node),
      newResumeTreeNode(NodeType.ListItem, 6, this.node),
    ];
  }
}
