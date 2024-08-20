import { NodeType, ResumeTreeNode } from '../../../../../models/Resume';
import {
  fireEvent,
  Guid,
  renderRootComponent,
  screen,
} from '../../../../../common/testing-imports';
import { SectionWorkExperienceComponent } from './section-work-experience.component';

describe('SectionWorkExperienceComponent', () => {
  let node: ResumeTreeNode;
  beforeEach(() => {
    node = {
      nodeType: NodeType.WorkExperience,
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
  it('should create child nodes on creation', async () => {
    const { fixture } = await render(node);

    expect(fixture.componentInstance.node.children.length).toBe(7);
    fixture.componentInstance.node.children.forEach((child, index) => {
      if (index < 6) expect(child.nodeType).toBe(NodeType.ListItem);
      else expect(child.nodeType).toBe(NodeType.List);
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

    fixture.componentInstance.ngOnChanges();

    expect(onSave).toHaveBeenCalledTimes(7);
  });
  it('should not emit saves if children already exist', async () => {
    const { fixture } = await render(node);
    const onSave = jest.spyOn(fixture.componentInstance.onSave, 'emit');

    fixture.componentInstance.ngOnChanges();

    expect(onSave).toHaveBeenCalledTimes(0);
  });
  describe('Changes', () => {
    let instance, onSave;
    beforeEach(async () => {
      const { fixture } = await render(node);
      onSave = jest.spyOn(fixture.componentInstance.onSave, 'emit');
      instance = fixture.componentInstance;
      fireEvent.input(screen.getByTitle('title'), {
        target: { value: 'title changed' },
      });
      fireEvent.input(screen.getByTitle('employer'), {
        target: { value: 'employer changed' },
      });
      fireEvent.input(screen.getByTitle('city'), {
        target: { value: 'city changed' },
      });
      fireEvent.input(screen.getByTitle('state'), {
        target: { value: 'state changed' },
      });
      fireEvent.input(screen.getByTitle('startDate'), {
        target: { value: '1999-08-08' },
      });
      fireEvent.input(screen.getByTitle('endDate'), {
        target: { value: '2000-08-08' },
      });
    });
    it('should update nodes on text updates', () => {
      expect(instance.node.children[0].content).toBe('title changed');
      expect(instance.node.children[1].content).toBe('employer changed');
      expect(instance.node.children[2].content).toBe('city changed');
      expect(instance.node.children[3].content).toBe('state changed');
      expect(instance.node.children[4].content).toBe('1999-08-08');
      expect(instance.node.children[5].content).toBe('2000-08-08');
    });
    it('should emit saves on text updates', () => {
      expect(onSave).toHaveBeenCalledTimes(6);
    });
  });
});

const render = async (node: ResumeTreeNode) => {
  return await renderRootComponent(SectionWorkExperienceComponent, {
    componentProperties: { node },
  });
};
