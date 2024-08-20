import { Component, Input, OnInit } from '@angular/core';
import {
  newResumeTreeNode,
  NodeType,
  ResumeTreeNode,
} from '../../../../../models/Resume';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { InputComponent } from '../../../../../common/input/input.component';
import {
  BorderStyle,
  ButtonComponent,
  ButtonStyle,
} from '../../../../../common/button/button.component';
import { UpsertSignal } from '../upsert-signal/upsert-signal';

@Component({
  selector: 'app-personal-info',
  standalone: true,
  imports: [ReactiveFormsModule, InputComponent, ButtonComponent],
  templateUrl: './personal-info.component.html',
  styleUrl: './personal-info.component.scss',
})
export class PersonalInfoComponent extends UpsertSignal implements OnInit {
  @Input() node: ResumeTreeNode;
  form: FormGroup;

  ngOnInit() {
    this.form = new FormGroup({
      name: new FormControl(this.node.content),
      email: new FormControl(this.node.children[0].content),
      phone: new FormControl(this.node.children[1].content),
    });

    this.form.controls['name'].valueChanges.subscribe(() => {
      this.node.content = this.form.controls['name'].value;
      this.queueSave(this.node);
    });
    this.form.controls['email'].valueChanges.subscribe(() => {
      this.node.children[0].content = this.form.controls['email'].value;
      this.queueSave(this.node.children[0]);
    });
    this.form.controls['phone'].valueChanges.subscribe(() => {
      this.node.children[1].content = this.form.controls['phone'].value;
      this.queueSave(this.node.children[1]);
    });

    if (this.node.children.length > 2) {
      this.AddWebsite(true);
    }
  }

  AddWebsite(existing: boolean = false) {
    this.form.addControl('website', new FormControl(''));

    if (!existing) {
      this.node.children.push(
        newResumeTreeNode(NodeType.ListItem, 2, this.node),
      );
    } else {
      this.form.get('website').setValue(this.node.children[2].content);
    }

    this.form.get('website').valueChanges.subscribe(() => {
      this.node.children[2].content = this.form.get('website').value;
      this.queueSave(this.node.children[2]);
    });

    this.queueSave(this.node.children[2]);
  }

  RemoveWebsite() {
    this.form.removeControl('website');
    const node = this.node.children.splice(2, 1);
    this.queueDelete(node[0].id);
  }

  protected readonly BorderStyle = BorderStyle;
  protected readonly ButtonStyle = ButtonStyle;
}
