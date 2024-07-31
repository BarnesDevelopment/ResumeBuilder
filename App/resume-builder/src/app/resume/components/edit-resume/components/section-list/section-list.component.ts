import { Component, Input } from '@angular/core';
import { ResumeTreeNode } from '../../../../../models/Resume';

@Component({
  selector: 'app-section-list',
  standalone: true,
  imports: [],
  templateUrl: './section-list.component.html',
  styleUrl: './section-list.component.scss',
})
export class SectionListComponent {
  @Input() node: ResumeTreeNode;
}
