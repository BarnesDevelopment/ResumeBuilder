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
  selector: 'app-section-list',
  template: '',
})
export class SectionListComponentStub {
  @Input() node: ResumeTreeNode;
}
