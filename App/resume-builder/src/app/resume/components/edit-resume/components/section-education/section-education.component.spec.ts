import {
  Guid,
  NodeType,
  renderRootComponent,
  ResumeTreeNode,
  screen,
  fireEvent,
} from '../../../../../common/testing-imports';
import { SectionEducationComponent } from './section-education.component';

describe('SectionEducationComponent', () => {
  let node: ResumeTreeNode;
  beforeEach(() => {
    node = {
      children: [],
      nodeType: NodeType.Education,
      id: Guid.create(),
      parentId: Guid.create(),
      content: '',
      comments: '',
      active: true,
      depth: 0,
      userId: Guid.create(),
      order: 0,
    };
  });
  it('should create child nodes on creation', async () => {
    const { fixture } = await render(node);

    expect(fixture.componentInstance.node.children.length).toBe(7);
    fixture.componentInstance.node.children.forEach((child, index) => {
      expect(child.nodeType).toBe(NodeType.ListItem);
      expect(child.order).toBe(index);
      expect(child.parentId).toBe(node.id);
      expect(child.depth).toBe(node.depth + 1);
      expect(child.content).toBe('');
    });
  });
  it('should emit saves on creation', async () => {
    const { fixture } = await render(node);
    fixture.componentInstance.node.children = [];
    const onSave = jest.spyOn(fixture.componentInstance.onSave, 'emit');

    fixture.componentInstance.ngOnInit();

    expect(onSave).toHaveBeenCalledTimes(7);
  });
  it('should not emit saves if children already exist', async () => {
    const { fixture } = await render(node);
    const onSave = jest.spyOn(fixture.componentInstance.onSave, 'emit');

    fixture.componentInstance.ngOnInit();

    expect(onSave).toHaveBeenCalledTimes(0);
  });
  it('should update nodes on text updates', async () => {
    const { fixture } = await render(node);

    fireEvent.input(screen.getByTitle('degree'), {
      target: { value: 'degree changed' },
    });
    fireEvent.input(screen.getByTitle('major'), {
      target: { value: 'major changed' },
    });
    fireEvent.input(screen.getByTitle('minor'), {
      target: { value: 'minor changed' },
    });
    fireEvent.input(screen.getByTitle('school'), {
      target: { value: 'school changed' },
    });
    fireEvent.input(screen.getByTitle('city'), {
      target: { value: 'city changed' },
    });
    fireEvent.input(screen.getByTitle('state'), {
      target: { value: 'state changed' },
    });
    fireEvent.input(screen.getByTitle('graduationYear'), {
      target: { value: '2244' },
    });

    expect(fixture.componentInstance.node.children[0].content).toBe(
      'degree changed',
    );
    expect(fixture.componentInstance.node.children[1].content).toBe(
      'major changed',
    );
    expect(fixture.componentInstance.node.children[2].content).toBe(
      'minor changed',
    );
    expect(fixture.componentInstance.node.children[3].content).toBe(
      'school changed',
    );
    expect(fixture.componentInstance.node.children[4].content).toBe(
      'city changed',
    );
    expect(fixture.componentInstance.node.children[5].content).toBe(
      'state changed',
    );
    expect(fixture.componentInstance.node.children[6].content).toBe('2244');
  });
  it('should emit saves on text updates', async () => {
    const { fixture } = await render(node);
    const onSave = jest.spyOn(fixture.componentInstance.onSave, 'emit');

    fireEvent.input(screen.getByTitle('degree'), {
      target: { value: 'degree changed' },
    });
    fireEvent.input(screen.getByTitle('major'), {
      target: { value: 'major changed' },
    });
    fireEvent.input(screen.getByTitle('minor'), {
      target: { value: 'minor changed' },
    });
    fireEvent.input(screen.getByTitle('school'), {
      target: { value: 'school changed' },
    });
    fireEvent.input(screen.getByTitle('city'), {
      target: { value: 'city changed' },
    });
    fireEvent.input(screen.getByTitle('state'), {
      target: { value: 'state changed' },
    });
    fireEvent.input(screen.getByTitle('graduationYear'), {
      target: { value: '2244' },
    });

    expect(onSave).toHaveBeenCalledTimes(7);
  });
});

const render = async (node: ResumeTreeNode) => {
  return await renderRootComponent(SectionEducationComponent, {
    componentProperties: {
      node,
    },
  });
};
