import { SectionListComponent } from './section-list.component';
import {
  Guid,
  NodeType,
  ResumeTreeNode,
  screen,
  renderRootComponent,
} from '../../../../../common/testing-imports';

describe('SectionListComponent', () => {
  let rootNode: ResumeTreeNode;

  beforeEach(() => {
    rootNode = {
      nodeType: NodeType.List,
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
      await render(rootNode);

      expect(screen.queryByRole('textbox')).toBeInTheDocument();
    });
    it('should show a plus button', async () => {
      await render(rootNode);

      expect(
        screen.queryByRole('button', { name: 'plus' }),
      ).toBeInTheDocument();
    });
    it('should not show a minus button', async () => {
      expect(
        screen.queryByRole('button', { name: 'minus' }),
      ).not.toBeInTheDocument();
    });
    it('should create node for first list item', async () => {
      const component = await render(rootNode);

      expect(component.fixture.componentInstance.node.children.length).toBe(1);
      expect(
        component.fixture.componentInstance.node.children[0].nodeType,
      ).toBe(NodeType.ListItem);
      expect(component.fixture.componentInstance.node.children[0].content).toBe(
        '',
      );
    });
  });
  describe('Enter Information', () => {
    it('should create a new input when the plus button is clicked', async () => {});
    it('should only show the plus on the last row', async () => {});
    it('should show a minus button on each row when there are two or more inputs', async () => {});
    it('should remove the current input when the minus button is clicked', async () => {});
  });
  describe('Load Data', () => {
    it('should load the existing data into as many rows as necessary', async () => {});
    it('should populate the form with text', async () => {});
  });
  describe('Save', () => {});
});

const render = async (node: ResumeTreeNode) => {
  return await renderRootComponent(SectionListComponent, {
    componentProperties: {
      node,
    },
  });
};
