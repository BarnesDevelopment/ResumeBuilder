import { Component, Input } from '@angular/core';
import { ResumeTreeNode } from '../../../../../models/Resume';
import { UpsertSignal } from '../upsert-signal/upsert-signal';

@Component({
  selector: 'app-section-education',
  standalone: true,
  imports: [],
  templateUrl: './section-education.component.html',
  styleUrl: './section-education.component.scss',
})
export class SectionEducationComponent extends UpsertSignal {
  @Input() node: ResumeTreeNode;
}
