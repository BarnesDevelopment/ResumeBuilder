import '@testing-library/jest-dom';
import { screen, fireEvent } from '@testing-library/angular';
import { renderRootComponent } from './RenderRootComponent';
import { NodeType, ResumeTreeNode } from '../models/Resume';
import { Guid } from 'guid-typescript';
import { Component, Input } from '@angular/core';

export {
  screen,
  fireEvent,
  renderRootComponent,
  NodeType,
  ResumeTreeNode,
  Guid,
};

@Component({
  standalone: true,
  selector: 'app-resume-section',
  template: '',
})
export class ResumeSectionComponentStub {
  @Input() section: ResumeTreeNode;
}

@Component({
  standalone: true,
  selector: 'app-section-list',
  template: '',
})
export class SectionListComponentStub {
  @Input() node: ResumeTreeNode;
}

@Component({
  standalone: true,
  selector: 'app-section-education',
  template: '',
})
export class SectionEducationComponentStub {
  @Input() node: ResumeTreeNode;
}

@Component({
  standalone: true,
  selector: 'app-section-work-experience',
  template: '',
})
export class SectionWorkExperienceComponentStub {
  @Input() node: ResumeTreeNode;
}

@Component({
  standalone: true,
  selector: 'app-personal-info',
  template: '',
})
export class PersonalInfoComponentStub {
  @Input() node: ResumeTreeNode;
}

@Component({
  standalone: true,
  selector: 'ngx-extended-pdf-viewer',
  template: '',
})
export class NgxExtendedPdfViewerComponentStub {
  @Input() src;
  @Input() height;
  @Input() textLayer;
}
