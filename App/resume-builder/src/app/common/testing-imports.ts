import '@testing-library/jest-dom';
import { screen, fireEvent } from '@testing-library/angular';
import { renderRootComponent } from './RenderRootComponent';
import { NodeType, ResumeTreeNode } from '../models/Resume';
import { Guid } from 'guid-typescript';

export {
  screen,
  fireEvent,
  renderRootComponent,
  NodeType,
  ResumeTreeNode,
  Guid,
};
