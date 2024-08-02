import { Component, Input } from '@angular/core';
import { ResumeTreeNode } from '../../../../../models/Resume';

@Component({
  selector: 'app-section-paragraph',
  standalone: true,
  imports: [],
  templateUrl: './section-paragraph.component.html',
  styleUrl: './section-paragraph.component.scss',
})
export class SectionParagraphComponent {
  @Input() node: ResumeTreeNode;
}
