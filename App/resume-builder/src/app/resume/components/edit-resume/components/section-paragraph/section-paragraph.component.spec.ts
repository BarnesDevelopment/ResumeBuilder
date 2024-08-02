import {
  fireEvent,
  Guid,
  NodeType,
  renderRootComponent,
  ResumeTreeNode,
  screen,
} from '../../../../../common/testing-imports';
import { SectionParagraphComponent } from './section-paragraph.component';
import { newResumeTreeNode } from '../../../../../models/Resume';

describe('SectionParagraphComponent', () => {
  let node: ResumeTreeNode;

  beforeEach(() => {
    node = {
      nodeType: NodeType.Paragraph,
      children: [],
      content: '',
      comments: '',
      id: Guid.create(),
      depth: 2,
      parentId: Guid.create(),
      active: true,
      userId: Guid.create(),
      order: 0,
    };
  });
  describe('Init', () => {
    it('should show a single empty input', async () => {
      const component = await render(node);

      expect(screen.queryByRole('textbox')).toBeInTheDocument();
    });
  });
  describe('Enter Information', () => {
    it('should update the node content when the input is changed', async () => {
      const component = await render(node);

      const input = component.queryByRole('textbox');
      fireEvent.input(input, { target: { value: 'Test' } });
      component.detectChanges();
      expect(component.fixture.componentInstance.node.content).toBe('Test');
    });
  });
  describe('Load Data', () => {
    it('should show the node content in the input', async () => {
      node.content = 'Test';
      const component = await render(node);

      const input = screen.getByRole('textbox') as HTMLInputElement;

      expect(input.value).toBe('Test');
    });
  });
  describe('Save', () => {});
});

const render = async (node: ResumeTreeNode) => {
  return await renderRootComponent(SectionParagraphComponent, {
    componentProperties: { node },
  });
};