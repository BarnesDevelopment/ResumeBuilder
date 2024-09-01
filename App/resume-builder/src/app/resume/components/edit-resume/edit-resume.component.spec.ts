import {
  fireEvent,
  Guid,
  NgxExtendedPdfViewerComponentStub,
  NodeType,
  PersonalInfoComponentStub,
  renderRootComponent,
  ResumeSectionComponentStub,
  ResumeTreeNode,
  screen,
} from '../../../common/testing-imports';
import { EditResumeComponent } from './edit-resume.component';
import { By } from '@angular/platform-browser';
import { ToastrService } from 'ngx-toastr';
import { ResumeService } from '../../services/resume.service';
import { Observable, of, throwError } from 'rxjs';
import { PersonalInfoComponent } from './components/personal-info/personal-info.component';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import { newResumeTreeNode } from '../../../models/Resume';
import { ResumeSectionComponent } from './components/resume-section/resume-section.component';

describe('EditResumeComponent', () => {
  let rootNode: ResumeTreeNode;
  let resumeServiceSpy;

  beforeEach(() => {
    jest.resetAllMocks();
    rootNode = {
      id: Guid.create(),
      nodeType: NodeType.Resume,
      userId: Guid.create(),
      parentId: null,
      content: 'Resume Name 1',
      active: true,
      order: 0,
      depth: 0,
      children: [
        {
          id: Guid.create(),
          nodeType: NodeType.Title,
          userId: Guid.create(),
          parentId: Guid.create(),
          content: 'Personal Info',
          active: true,
          order: 0,
          depth: 1,
          children: [],
          comments: '',
        },
      ],
      comments: '',
    };
    resumeServiceSpy = jest
      .spyOn(ResumeService.prototype, 'getResume')
      .mockReturnValue(of(rootNode));
  });
  describe('Init', () => {
    it('should show title and comments', async () => {
      rootNode.comments = 'comments';
      await render();

      // screen.debug(undefined, 10000);
      expect((screen.getByTitle('title') as HTMLInputElement).value).toBe(
        'Resume Name 1',
      );
      expect((screen.getByTitle('comments') as HTMLInputElement).value).toBe(
        'comments',
      );
    });
    describe('Sections', () => {
      it('should show add section button', async () => {
        await render();

        expect(screen.queryByText('Add Section')).toBeTruthy();
      });
      it('should not show any sections', async () => {
        const component = await render();

        expect(
          component.debugElement.queryAll(By.css('app-resume-section')).length,
        ).toBe(0);
      });
    });
  });
  describe('Edit', () => {
    describe('Title', () => {
      it('should update title', async () => {
        const component = await render();
        const title = screen.getByTitle('title');
        fireEvent.input(title, { target: { value: 'New Title' } });

        expect(component.fixture.componentInstance.resume.content).toBe(
          'New Title',
        );
      });
      it('should update comments', async () => {
        const component = await render();
        const comments = screen.getByTitle('comments');
        fireEvent.input(comments, { target: { value: 'New Comments' } });

        expect(component.fixture.componentInstance.resume.comments).toBe(
          'New Comments',
        );
      });
      describe('Save', () => {
        it('should queue save on title change', async () => {
          const component = await render();
          const title = screen.getByTitle('title');
          fireEvent.input(title, { target: { value: 'New Title' } });

          expect(component.fixture.componentInstance.saves[0].content).toBe(
            'New Title',
          );
        });
        it('should queue save on comments change', async () => {
          const component = await render();
          const comments = screen.getByTitle('comments');
          fireEvent.input(comments, { target: { value: 'New Comments' } });

          expect(component.fixture.componentInstance.saves[0].comments).toBe(
            'New Comments',
          );
        });
      });
    });
    describe('Sections', () => {
      describe('Add', () => {
        it('should add section', async () => {
          const component = await render();

          fireEvent.click(screen.getByText('Add Section'));

          expect(
            component.debugElement.queryAll(By.css('app-resume-section'))
              .length,
          ).toBe(1);
          expect(
            component.fixture.componentInstance.resume.children.length,
          ).toBe(2);
          expect(
            component.fixture.componentInstance.resume.children[1].nodeType,
          ).toBe(NodeType.Section);
          expect(
            component.fixture.componentInstance.resume.children[1].children
              .length,
          ).toBe(0);
        });
      });

      describe('Remove', () => {
        it('should remove section', async () => {
          const component = await render();

          fireEvent.click(screen.getByText('Add Section'));
          fireEvent.click(screen.getByText('Add Section'));

          expect(screen.getAllByTestId('section').length).toBe(2);

          fireEvent.click(screen.getAllByText('Remove Section')[0]);

          expect(screen.getAllByTestId('section').length).toBe(1);
        });

        it('should reorder sections', async () => {
          const component = await render();

          fireEvent.click(screen.getByText('Add Section'));
          fireEvent.click(screen.getByText('Add Section'));
          fireEvent.click(screen.getByText('Add Section'));

          const ids = [
            component.fixture.componentInstance.resume.children[0].id,
            component.fixture.componentInstance.resume.children[2].id,
            component.fixture.componentInstance.resume.children[3].id,
          ];

          fireEvent.click(screen.getAllByText('Remove Section')[0]);

          component.fixture.componentInstance.resume.children.forEach(
            (child, index) => {
              expect(child.order).toBe(index);
              expect(child.id).toBe(ids[index]);
            },
          );
        });

        it('should queue delete on section removal', async () => {
          const component = await render();

          fireEvent.click(screen.getByText('Add Section'));

          const expectedId =
            component.fixture.componentInstance.resume.children[1].id;

          fireEvent.click(screen.getAllByText('Remove Section')[0]);

          expect(component.fixture.componentInstance.deletes[0]).toBe(
            expectedId,
          );
        });
      });
    });
  });
  //TODO: PDF Preview
  describe('Save', () => {
    let toasterErrorSpy, toasterSuccessSpy, updateSpy, deleteSpy;

    beforeEach(() => {
      updateSpy = jest.spyOn(ResumeService.prototype, 'updateResume');
      deleteSpy = jest.spyOn(ResumeService.prototype, 'deleteNode');
      toasterSuccessSpy = jest.spyOn(ToasterServiceMock.prototype, 'success');
      toasterErrorSpy = jest.spyOn(ToasterServiceMock.prototype, 'error');
    });
    describe('Save lists', () => {
      it('should add item to saves list', async () => {
        const save = newResumeTreeNode(NodeType.Section, 0, rootNode);
        save.content = 'New Section';
        const { fixture } = await render();

        fixture.componentInstance.queueSave(save);

        expect(fixture.componentInstance.saves[0]).toBe(save);
      });
      it('should add item to deletes list', async () => {
        const deleteId = Guid.create();
        const { fixture } = await render();

        fixture.componentInstance.queueDelete(deleteId);

        expect(fixture.componentInstance.deletes[0]).toBe(deleteId);
      });
      it('should update item in saves list', async () => {
        const save = newResumeTreeNode(NodeType.Section, 0, rootNode);
        save.content = 'New Section';
        const updatedSave = {
          ...save,
          content: 'Updated Section',
        };
        const { fixture } = await render([save]);

        fixture.componentInstance.queueSave(updatedSave);

        expect(fixture.componentInstance.saves[0]).toBe(updatedSave);
        expect(fixture.componentInstance.saves.length).toBe(1);
      });
    });
    it('should run all saves when there are no deletes', async () => {
      const saves = [
        newResumeTreeNode(NodeType.Section, 0, rootNode),
        newResumeTreeNode(NodeType.Section, 1, rootNode),
      ];
      updateSpy.mockReturnValue(of());
      await render(saves);

      const saveButton = screen.getByText('Save');
      saveButton.click();

      expect(updateSpy).toHaveBeenCalledTimes(1);
      expect(updateSpy).toHaveBeenCalledWith([saves[0], saves[1]]);
    });
    it('should run all deletes', async () => {
      const deletes = [Guid.create(), Guid.create()];
      deleteSpy.mockReturnValue(of());
      await render([], deletes);

      const saveButton = screen.getByText('Save');
      saveButton.click();

      expect(deleteSpy).toHaveBeenCalledTimes(2);
      expect(deleteSpy).toHaveBeenCalledWith(deletes[0]);
      expect(deleteSpy).toHaveBeenCalledWith(deletes[1]);
    });
    it('should remove deletes from saves before saving', async () => {
      const deletes = [Guid.create(), Guid.create()];
      const saves = [
        newResumeTreeNode(NodeType.Section, 0, rootNode),
        newResumeTreeNode(NodeType.Section, 1, rootNode),
      ];
      saves[0].id = deletes[0];

      updateSpy.mockReturnValue(of());
      deleteSpy.mockReturnValue(of());
      await render(saves, deletes);

      const saveButton = screen.getByText('Save');
      saveButton.click();

      expect(updateSpy).toHaveBeenCalledTimes(1);
      expect(updateSpy).toHaveBeenCalledWith([saves[1]]);
      expect(deleteSpy).toHaveBeenCalledTimes(2);
      expect(deleteSpy).toHaveBeenCalledWith(deletes[0]);
      expect(deleteSpy).toHaveBeenCalledWith(deletes[1]);
    });
    it('should show success toaster on successful save', async () => {
      const saves = [
        newResumeTreeNode(NodeType.Section, 0, rootNode),
        newResumeTreeNode(NodeType.Section, 1, rootNode),
      ];

      updateSpy
        .mockReturnValueOnce(of(saves[0]))
        .mockReturnValueOnce(of(saves[1]));
      await render(saves);

      const saveButton = screen.getByText('Save');
      saveButton.click();

      expect(toasterErrorSpy).not.toHaveBeenCalled();
      expect(toasterSuccessSpy).toHaveBeenCalledWith(
        'Resume saved successfully',
        'Saved',
      );
    });
    it('should show error toaster on failed save', async () => {
      const saves = [
        newResumeTreeNode(NodeType.Section, 0, rootNode),
        newResumeTreeNode(NodeType.Section, 1, rootNode),
      ];

      updateSpy
        .mockImplementationOnce(() => {
          return throwError(() => new Error('error'));
        })
        .mockReturnValueOnce(of(saves[1]));

      await render(saves);

      const saveButton = screen.getByText('Save');
      saveButton.click();

      expect(toasterSuccessSpy).not.toHaveBeenCalled();
      expect(toasterErrorSpy).toHaveBeenCalledWith(
        'Error saving resume: Error: error',
        'Error',
      );
    });
    it('should do nothing if there is nothing to save', async () => {
      await render();

      const saveButton = screen.getByText('Save');
      saveButton.click();

      expect(updateSpy).not.toHaveBeenCalled();
      expect(deleteSpy).not.toHaveBeenCalled();
      expect(toasterSuccessSpy).not.toHaveBeenCalled();
      expect(toasterErrorSpy).not.toHaveBeenCalled();
    });
    it('should clear lists after saving successfully', async () => {
      const saves = [
        newResumeTreeNode(NodeType.Section, 0, rootNode),
        newResumeTreeNode(NodeType.Section, 1, rootNode),
      ];

      updateSpy
        .mockReturnValueOnce(of(saves[0]))
        .mockReturnValueOnce(of(saves[1]));
      const { fixture } = await render(saves);

      const saveButton = screen.getByText('Save');
      saveButton.click();

      fixture.detectChanges();

      expect(fixture.componentInstance.saves.length).toBe(0);
      expect(fixture.componentInstance.deletes.length).toBe(0);
    });
    it('should save in order', async () => {
      const saves = [
        newResumeTreeNode(NodeType.Section, 1, rootNode),
        newResumeTreeNode(NodeType.Section, 0, rootNode),
        newResumeTreeNode(NodeType.Section, 2, rootNode),
      ];

      updateSpy
        .mockReturnValueOnce(of(saves[1]))
        .mockReturnValueOnce(of(saves[0]))
        .mockReturnValueOnce(of(saves[2]));
      await render(saves);

      const saveButton = screen.getByText('Save');
      saveButton.click();

      const updateCalls = updateSpy.mock.calls[0][0];
      expect(updateCalls.length).toBe(3);
      expect(updateCalls[0]).toBe(saves[1]);
      expect(updateCalls[1]).toBe(saves[0]);
      expect(updateCalls[2]).toBe(saves[2]);
    });
    it('should save in order of depth', async () => {
      const saves = [
        newResumeTreeNode(NodeType.Section, 0, rootNode), // depth 2
        newResumeTreeNode(NodeType.Section, 1, rootNode), // depth 3
        newResumeTreeNode(NodeType.Section, 2, rootNode), // depth 1
      ];
      saves[0].depth = 2;
      saves[1].depth = 3;

      updateSpy
        .mockReturnValueOnce(of(saves[2]))
        .mockReturnValueOnce(of(saves[0]))
        .mockReturnValueOnce(of(saves[1]));
      await render(saves);

      const saveButton = screen.getByText('Save');
      saveButton.click();

      const updateCalls = updateSpy.mock.calls[0][0];
      expect(updateCalls.length).toBe(3);
      expect(updateCalls[0]).toBe(saves[2]);
      expect(updateCalls[1]).toBe(saves[0]);
      expect(updateCalls[2]).toBe(saves[1]);
    });
    it('should save by depth and then order', async () => {
      const saves = [
        newResumeTreeNode(NodeType.Section, 1, rootNode), // depth 2
        newResumeTreeNode(NodeType.Section, 0, rootNode), // depth 2
        newResumeTreeNode(NodeType.Section, 0, rootNode), // depth 3
        newResumeTreeNode(NodeType.Section, 3, rootNode), // depth 1
        newResumeTreeNode(NodeType.Section, 4, rootNode), // depth 1
      ];
      saves[0].depth = 2;
      saves[1].depth = 2;
      saves[2].depth = 3;
      saves[3].depth = 1;
      saves[4].depth = 1;

      updateSpy
        .mockReturnValueOnce(of(saves[3]))
        .mockReturnValueOnce(of(saves[4]))
        .mockReturnValueOnce(of(saves[1]))
        .mockReturnValueOnce(of(saves[0]))
        .mockReturnValueOnce(of(saves[2]));
      await render(saves);

      const saveButton = screen.getByText('Save');
      saveButton.click();

      const updateCalls = updateSpy.mock.calls[0][0];
      expect(updateCalls.length).toBe(5);
      expect(updateCalls[0]).toBe(saves[3]);
      expect(updateCalls[1]).toBe(saves[4]);
      expect(updateCalls[2]).toBe(saves[1]);
      expect(updateCalls[3]).toBe(saves[0]);
      expect(updateCalls[4]).toBe(saves[2]);
    });
  });
});

const render = async (saves: ResumeTreeNode[] = [], deletes: Guid[] = []) => {
  return await renderRootComponent(EditResumeComponent, {
    componentProperties: {
      saves,
      deletes,
    },
    providers: [
      {
        provide: ToastrService,
        useClass: ToasterServiceMock,
      },
    ],
    importOverrides: [
      { remove: PersonalInfoComponent, add: PersonalInfoComponentStub },
      {
        remove: NgxExtendedPdfViewerModule,
        add: NgxExtendedPdfViewerComponentStub,
      },
      {
        remove: ResumeSectionComponent,
        add: ResumeSectionComponentStub,
      },
    ],
  });
};

class ToasterServiceMock {
  success() {}

  error() {}
}
