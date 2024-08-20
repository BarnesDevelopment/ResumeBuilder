import { Component, Input, OnChanges } from '@angular/core';
import {
  newResumeTreeNode,
  NodeType,
  ResumeTreeNode,
} from '../../../../../models/Resume';
import { InputComponent } from '../../../../../common/input/input.component';
import { FormArray, FormControl, ReactiveFormsModule } from '@angular/forms';
import {
  ButtonComponent,
  ButtonStyle,
} from '../../../../../common/button/button.component';
import { UpsertSignal } from '../upsert-signal/upsert-signal';

@Component({
  selector: 'app-section-list',
  standalone: true,
  imports: [InputComponent, ReactiveFormsModule, ButtonComponent],
  templateUrl: './section-list.component.html',
  styleUrl: './section-list.component.scss',
})
export class SectionListComponent extends UpsertSignal implements OnChanges {
  @Input() node: ResumeTreeNode;
  form: FormArray<FormControl<string>>;

  ngOnChanges() {
    this.form = new FormArray([]);
    if (!this.node.children.length) this.AddListItem();
    else {
      this.node.children.forEach(child => {
        this.AddFormListItem(child.content, child.order);
      });
    }
  }

  AddListItem() {
    const order = this.node.children.length;
    this.node.children.push(
      newResumeTreeNode(NodeType.ListItem, order, this.node),
    );
    this.queueSave(this.node.children[0]);
    this.AddFormListItem('', order);
  }

  AddFormListItem(content: string, order: number) {
    this.form.push(new FormControl(content));
    this.form.controls[order].valueChanges.subscribe(value => {
      this.node.children[order].content = value;
      this.queueSave(this.node.children[order]);
    });
  }

  protected readonly ButtonStyle = ButtonStyle;

  AddItem() {
    this.AddListItem();
  }

  RemoveItem($index: number) {
    const removed = this.node.children.splice($index, 1);
    this.form.removeAt($index);
    this.UpdateOrder();
    this.queueDelete(removed[0].id);
  }

  UpdateOrder() {
    this.node.children.forEach((child, index) => {
      if (child.order !== index) {
        child.order = index;
        this.queueSave(child);
      }
    });
  }
}
