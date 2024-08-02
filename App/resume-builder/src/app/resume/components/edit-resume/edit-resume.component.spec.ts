import {
  Guid,
  NodeType,
  renderRootComponent,
  ResumeTreeNode,
} from '../../../common/testing-imports';
import { EditResumeComponent } from './edit-resume.component';

describe('EditResumeComponent', () => {
  let rootNode: ResumeTreeNode;

  beforeEach(() => {
    rootNode = {
      id: Guid.create(),
      nodeType: NodeType.Resume,
      userId: Guid.create(),
      parentId: null,
      content: 'Resume Name 1',
      active: true,
      order: 0,
      depth: 0,
      children: [],
      comments: '',
    };
  });
  describe('Init', () => {
    describe('Personal Info', () => {});
    describe('Sections', () => {
      //add section button
    });
  });
  describe('Load Data', () => {
    describe('Sections', () => {
      //load sections
    });
  });
  describe('Edit', () => {
    describe('Title', () => {
      //edit title
      //edit comments
    });
    describe('Sections', () => {
      //add section
      //remove section
    });
  });
  describe('Save', () => {
    //save collection
    //delete collection
    //execute saves and deletes
  });
});

const render = async () => {
  return await renderRootComponent(EditResumeComponent, {});
};
