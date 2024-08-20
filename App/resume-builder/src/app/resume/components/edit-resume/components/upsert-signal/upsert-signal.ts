import { Component, output } from '@angular/core';
import { ResumeTreeNode } from '../../../../../models/Resume';
import { Guid } from 'guid-typescript';

@Component({
  standalone: true,
  template: '',
})
export class UpsertSignal {
  onSave = output<ResumeTreeNode>();
  onDelete = output<Guid>();

  queueSave(node: ResumeTreeNode) {
    this.onSave.emit({ ...node, children: [] });
  }

  queueDelete(id: Guid) {
    this.onDelete.emit(id);
  }

  queueDeleteRecursive(node: ResumeTreeNode) {
    this.onDelete.emit(node.id);
    node.children.forEach(child => this.queueDeleteRecursive(child));
  }
}
