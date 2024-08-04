import { Component, Input } from '@angular/core';
import { ResumeTreeNode } from '../../../../../models/Resume';
import { UpsertSignal } from '../upsert-signal/upsert-signal';

@Component({
  selector: 'app-section-work-experience',
  standalone: true,
  imports: [],
  templateUrl: './section-work-experience.component.html',
  styleUrl: './section-work-experience.component.scss',
})
export class SectionWorkExperienceComponent extends UpsertSignal {
  @Input() node: ResumeTreeNode;
}
