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
import { of } from 'rxjs';
import { PersonalInfoComponent } from './components/personal-info/personal-info.component';
import {
  NgxExtendedPdfViewerComponent,
  NgxExtendedPdfViewerModule,
} from 'ngx-extended-pdf-viewer';

describe('EditResumeComponent', () => {
  let rootNode: ResumeTreeNode;
  let resumeServiceSpy;

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
    //execute saves and deletes
  });
});

const render = async () => {
  return await renderRootComponent(EditResumeComponent, {
    providers: [
      {
        provide: ToastrService,
        useValue: {
          success: () => {},
          error: () => {},
        },
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
