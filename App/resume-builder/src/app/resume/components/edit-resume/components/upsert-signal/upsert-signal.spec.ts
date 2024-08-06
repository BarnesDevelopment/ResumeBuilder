import { UpsertSignal } from './upsert-signal';
import {
  newResumeTreeNode,
  NodeType,
  ResumeTreeNode,
} from '../../../../../models/Resume';
import {
  renderRootComponent,
  Guid,
} from '../../../../../common/testing-imports';

describe('UpsertSignal', () => {
  const root: ResumeTreeNode = {
    id: Guid.create(),
    userId: Guid.create(),
    parentId: Guid.create(),
    order: 0,
    depth: 0,
    content: 'root',
    nodeType: NodeType.Resume,
    active: true,
    children: [],
    comments: '',
  };

  let saveSpy, deleteSpy;

  it('should emit delete when queued', async () => {
    const { fixture } = await renderRootComponent(UpsertSignal);
    deleteSpy = jest.spyOn(fixture.componentInstance.onDelete, 'emit');
    const node = newResumeTreeNode(NodeType.Education, 0, root);
    fixture.componentInstance.queueDelete(node.id);

    expect(deleteSpy).toHaveBeenCalledTimes(1);
    expect(deleteSpy).toHaveBeenCalledWith(node.id);
  });
  it('should emit save when queued', async () => {
    const { fixture } = await renderRootComponent(UpsertSignal);
    saveSpy = jest.spyOn(fixture.componentInstance.onSave, 'emit');
    const node = newResumeTreeNode(NodeType.Education, 0, root);
    fixture.componentInstance.queueSave(node);

    expect(saveSpy).toHaveBeenCalledTimes(1);
    expect(saveSpy).toHaveBeenCalledWith(node);
  });
});
