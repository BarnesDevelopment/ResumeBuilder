import {
  fireEvent,
  Guid,
  NgxExtendedPdfViewerComponentStub,
  NodeType,
  PersonalInfoComponentStub,
  renderRootComponent,
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
import 'jest';

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
  describe('Load Data', () => {
    describe('Sections', () => {
      //load sections
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
    });
    describe('Sections', () => {
      it('should add section', async () => {
        const component = await render();
        const addSectionButton = screen.getByText('Add Section');

        addSectionButton.click();
        component.fixture.detectChanges();

        expect(
          component.debugElement.queryAll(By.css('app-resume-section')).length,
        ).toBe(1);
        expect(component.fixture.componentInstance.resume.children.length).toBe(
          2,
        );
        expect(
          component.fixture.componentInstance.resume.children[1].nodeType,
        ).toBe(NodeType.Section);
        expect(
          component.fixture.componentInstance.resume.children[1].children
            .length,
        ).toBe(0);
      });

      it('should remove section', async () => {
        //TODO: add remove functionality
      });
      //add section
      //remove section
    });
  });
  //TODO: PDF Preview
  describe('Save', () => {
    //save collection
    //delete collection
    let toasterErrorSpy, toasterSuccessSpy, updateSpy, deleteSpy;

    beforeEach(() => {
      updateSpy = jest.spyOn(ResumeService.prototype, 'updateResume');
      deleteSpy = jest.spyOn(ResumeService.prototype, 'deleteNode');
      toasterSuccessSpy = jest.spyOn(ToasterServiceMock.prototype, 'success');
      toasterErrorSpy = jest.spyOn(ToasterServiceMock.prototype, 'error');
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

      expect(updateSpy).toHaveBeenCalledTimes(2);
      expect(updateSpy).toHaveBeenCalledWith(saves[0]);
      expect(updateSpy).toHaveBeenCalledWith(saves[1]);
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
      expect(updateSpy).toHaveBeenCalledWith(saves[1]);
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
    ],
  });
};

class ToasterServiceMock {
  success() {}

  error() {}
}
