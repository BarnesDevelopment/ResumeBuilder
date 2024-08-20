import { SectionListComponent } from './section-list.component';
import {
  Guid,
  NodeType,
  ResumeTreeNode,
  screen,
  fireEvent,
  renderRootComponent,
} from '../../../../../common/testing-imports';
import { within } from '@testing-library/angular';
import { newResumeTreeNode } from '../../../../../models/Resume';

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

      expect(screen.queryByTitle('plusButton')).toBeInTheDocument();
    });
    it('should not show a minus button', async () => {
      await render(rootNode);

      expect(screen.queryByTitle('minusButton')).not.toBeInTheDocument();
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
    it('should create a new input when the plus button is clicked', async () => {
      await render(rootNode);

      const plusButton = screen.queryByTitle('plusButton');
      fireEvent.click(plusButton);

      expect(screen.queryAllByRole('textbox').length).toBe(2);
    });
    it('should only show the plus on the last row', async () => {
      await render(rootNode);

      const plusButton = screen.queryByTitle('plusButton');
      fireEvent.click(plusButton);

      expect(screen.queryAllByTitle('plusButton').length).toBe(1);

      screen.queryAllByTestId('list-item').forEach((item, index) => {
        if (index === 0) {
          expect(within(item).queryByTitle('plusButton')).toBeFalsy();
        } else {
          expect(within(item).queryByTitle('plusButton')).toBeTruthy();
        }
      });
    });
    it('should show a minus button on each row when there are two or more inputs', async () => {
      await render(rootNode);

      const plusButton = screen.queryByTitle('plusButton');
      fireEvent.click(plusButton);

      expect(screen.queryAllByTitle('minusButton').length).toBe(2);
    });
    it('should remove the current input when the minus button is clicked', async () => {
      const component = await render(rootNode);

      const plusButton = screen.queryByTitle('plusButton');
      fireEvent.click(plusButton);

      const minusButton = screen.queryAllByTitle('minusButton')[0];
      fireEvent.click(minusButton);

      expect(screen.queryAllByRole('textbox').length).toBe(1);
    });
    it('should remove the current input from node tree when the minus button is clicked', async () => {
      const component = await render(rootNode);

      const plusButton = screen.queryByTitle('plusButton');
      fireEvent.click(plusButton);

      screen.queryAllByRole('textbox').forEach((input, index) => {
        fireEvent.input(input, {
          target: { value: `Hello, World #${index}!` },
        });
      });

      const minusButton = screen.queryAllByTitle('minusButton')[0];
      fireEvent.click(minusButton);

      expect(component.fixture.componentInstance.node.children[0].content).toBe(
        'Hello, World #1!',
      );
    });
    it('should update order when item is deleted', async () => {
      const component = await render(rootNode);

      fireEvent.click(screen.queryByTitle('plusButton'));
      fireEvent.click(screen.queryByTitle('plusButton'));

      screen.queryAllByRole('textbox').forEach((input, index) => {
        fireEvent.input(input, {
          target: { value: `Hello, World #${index}!` },
        });
      });

      const minusButton = screen.queryAllByTitle('minusButton')[0];
      fireEvent.click(minusButton);

      expect(component.fixture.componentInstance.node.children[0].order).toBe(
        0,
      );
      expect(component.fixture.componentInstance.node.children[1].order).toBe(
        1,
      );
    });
    it('should update the node content when input is changed', async () => {
      const component = await render(rootNode);

      const input = screen.queryByRole('textbox');
      fireEvent.input(input, { target: { value: 'Hello, World!' } });

      expect(component.fixture.componentInstance.node.children[0].content).toBe(
        'Hello, World!',
      );
    });
  });
  describe('Load Data', () => {
    beforeEach(() => {
      rootNode.children = [
        newResumeTreeNode(NodeType.ListItem, 0, rootNode, 'Hello, World #0!'),
        newResumeTreeNode(NodeType.ListItem, 1, rootNode, 'Hello, World #1!'),
        newResumeTreeNode(NodeType.ListItem, 2, rootNode, 'Hello, World #2!'),
        newResumeTreeNode(NodeType.ListItem, 3, rootNode, 'Hello, World #3!'),
      ];
    });
    it('should load the existing data into as many rows as necessary', async () => {
      const component = await render(rootNode);

      component.fixture.componentInstance.form.controls.forEach(
        (input, index) => {
          expect(input.value).toBe(`Hello, World #${index}!`);
        },
      );
    });
    it('should populate the form with text', async () => {
      await render(rootNode);

      expect(screen.queryAllByRole('textbox').length).toBe(4);
      screen.queryAllByRole('textbox').forEach((input, index) => {
        expect(input).toHaveValue(`Hello, World #${index}!`);
      });
    });
  });
  describe('Save', () => {
    let saveSpy, deleteSpy;
    it('should emit save when adding list item', async () => {
      rootNode.children.push(newResumeTreeNode(NodeType.ListItem, 0, rootNode));
      const { fixture } = await render(rootNode);
      saveSpy = jest.spyOn(fixture.componentInstance.onSave, 'emit');
      const plusButton = screen.queryByTitle('plusButton');
      fireEvent.click(plusButton);

      expect(saveSpy).toHaveBeenCalledTimes(1);
      const node = saveSpy.mock.calls[0][0];
      expect(node.parentId).toBe(rootNode.id);
      expect(node.nodeType).toBe(NodeType.ListItem);
      expect(node.content).toBe('');
    });
    it('should emit save when order is updated', async () => {
      rootNode.children.push(newResumeTreeNode(NodeType.ListItem, 0, rootNode));
      rootNode.children.push(newResumeTreeNode(NodeType.ListItem, 1, rootNode));
      const { fixture } = await render(rootNode);
      saveSpy = jest.spyOn(fixture.componentInstance.onSave, 'emit');
      deleteSpy = jest.spyOn(fixture.componentInstance.onDelete, 'emit');
      const plusButton = screen.queryByTitle('plusButton');
      fireEvent.click(plusButton);
      const minusButton = screen.queryAllByTitle('minusButton')[1];
      fireEvent.click(minusButton);

      expect(saveSpy).toHaveBeenCalledTimes(2);
      expect(deleteSpy).toHaveBeenCalledTimes(1);
    });
    it('should emit save when updating list item text', async () => {
      rootNode.children.push(newResumeTreeNode(NodeType.ListItem, 0, rootNode));
      const { fixture } = await render(rootNode);
      saveSpy = jest.spyOn(fixture.componentInstance.onSave, 'emit');
      const input = screen.queryByRole('textbox');
      fireEvent.input(input, { target: { value: 'Hello, World!' } });

      expect(saveSpy).toHaveBeenCalledTimes(1);
      const node = saveSpy.mock.calls[0][0];
      expect(node.parentId).toBe(rootNode.id);
      expect(node.nodeType).toBe(NodeType.ListItem);
      expect(node.content).toBe('Hello, World!');
    });
    it('should emit save on initial load', async () => {
      const { fixture } = await render(rootNode);
      saveSpy = jest.spyOn(fixture.componentInstance.onSave, 'emit');
      fixture.componentInstance.node.children = [];
      fixture.componentInstance.ngOnChanges();

      expect(saveSpy).toHaveBeenCalledTimes(1);
      const node = saveSpy.mock.calls[0][0];
      expect(node.parentId).toBe(rootNode.id);
      expect(node.nodeType).toBe(NodeType.ListItem);
      expect(node.content).toBe('');
    });
    it('should not emit save when list items exist', async () => {
      rootNode.children.push(newResumeTreeNode(NodeType.ListItem, 0, rootNode));
      const { fixture } = await render(rootNode);
      saveSpy = jest.spyOn(fixture.componentInstance.onSave, 'emit');

      expect(saveSpy).toHaveBeenCalledTimes(0);
    });
    it('should emit delete when removing list item', async () => {
      const { fixture } = await render(rootNode);
      deleteSpy = jest.spyOn(fixture.componentInstance.onDelete, 'emit');

      const plusButton = screen.queryByTitle('plusButton');
      fireEvent.click(plusButton);
      const minusButton = screen.queryAllByTitle('minusButton')[0];
      fireEvent.click(minusButton);

      expect(deleteSpy).toHaveBeenCalledTimes(1);
    });
  });
});

const render = async (node: ResumeTreeNode) => {
  return await renderRootComponent(SectionListComponent, {
    componentProperties: {
      node,
    },
  });
};
