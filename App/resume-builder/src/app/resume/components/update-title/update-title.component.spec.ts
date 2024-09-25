import {
  fireEvent,
  Guid,
  renderRootComponent,
  ResumeTreeNode,
  screen,
} from '../../../common/testing-imports';
import { UpdateTitleComponent } from './update-title.component';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ResumeService } from '../../services/resume.service';
import { of } from 'rxjs';
import { Router } from '@angular/router';
import { EditResumeComponent } from '../edit-resume/edit-resume.component';

describe('UpdateTitleComponent', () => {
  let data,
    initialId,
    newId,
    getResumeSpy,
    duplicateResumeSpy,
    updateResumeSpy,
    routerSpy,
    closeSpy;

  beforeEach(() => {
    initialId = Guid.create();
    newId = Guid.create();
    data = { id: initialId, next: 3 };
    getResumeSpy = jest
      .spyOn(ResumeService.prototype, 'getResume')
      .mockReturnValue(
        of({
          id: initialId,
          comments: 'initial comment',
          content: 'title text',
          order: 1,
        } as ResumeTreeNode),
      )
      .mockReturnValue(
        of({
          id: newId,
          comments: 'initial comment',
          content: 'title text',
          order: 1,
        } as ResumeTreeNode),
      );
    duplicateResumeSpy = jest
      .spyOn(ResumeService.prototype, 'duplicateResume')
      .mockReturnValue(of(newId.toString()));
    updateResumeSpy = jest
      .spyOn(ResumeService.prototype, 'updateResume')
      .mockReturnValue(of({} as unknown as void));
    routerSpy = jest.spyOn(Router.prototype, 'navigate');
    closeSpy = jest.spyOn(MockDialogRef.prototype, 'close');
  });

  it('should populate the form with the resume data', async () => {
    await render(data);

    expect(getResumeSpy).toHaveBeenCalledWith(initialId.toString());
    expect(screen.getByLabelText('Title:')).toHaveValue('title text');
    expect(screen.getByLabelText('Comments:')).toHaveValue('initial comment');
  });

  describe('Save', () => {
    it('should duplicate the resume with new title and comments', async () => {
      await render(data);

      fireEvent.input(screen.getByLabelText('Title:'), {
        target: { value: 'new title' },
      });
      fireEvent.input(screen.getByLabelText('Comments:'), {
        target: { value: 'new comment' },
      });
      fireEvent.click(screen.getByText('Create'));

      expect(duplicateResumeSpy).toHaveBeenCalledWith(initialId.toString());
      expect(getResumeSpy).toHaveBeenCalledWith(newId.toString());
      expect(updateResumeSpy).toHaveBeenCalledWith([
        {
          id: newId,
          content: 'new title',
          comments: 'new comment',
          order: 3,
        },
      ]);
    });
    it('should close the dialog after saving', async () => {
      await render(data);

      fireEvent.input(screen.getByLabelText('Title:'), {
        target: { value: 'new title' },
      });
      fireEvent.input(screen.getByLabelText('Comments:'), {
        target: { value: 'new comment' },
      });
      fireEvent.click(screen.getByText('Create'));

      expect(closeSpy).toHaveBeenCalled();
    });
    it('should route to edit page after saving', async () => {
      await render(data);

      fireEvent.input(screen.getByLabelText('Title:'), {
        target: { value: 'new title' },
      });
      fireEvent.input(screen.getByLabelText('Comments:'), {
        target: { value: 'new comment' },
      });
      fireEvent.click(screen.getByText('Create'));

      expect(routerSpy).toHaveBeenCalledWith(['/edit', newId.toString()]);
    });
  });

  describe('Cancel', () => {
    it('should close the dialog after canceling', async () => {
      await render(data);

      fireEvent.click(screen.getByText('Cancel'));
      expect(closeSpy).toHaveBeenCalled();
    });
    it('should not duplicate if canceling', async () => {
      await render(data);

      fireEvent.click(screen.getByText('Cancel'));
      expect(duplicateResumeSpy).not.toHaveBeenCalled();
      expect(updateResumeSpy).not.toHaveBeenCalled();
    });
  });

  describe('Form validation', () => {
    describe('Title', () => {
      it('should not duplicate if title is empty', async () => {
        const component = await render(data);

        fireEvent.input(screen.getByLabelText('Title:'), {
          target: { value: '' },
        });
        fireEvent.click(screen.getByText('Create'));

        expect(component.fixture.componentInstance.saveDisabled).toBe(true);
        expect(duplicateResumeSpy).not.toHaveBeenCalled();
        expect(updateResumeSpy).not.toHaveBeenCalled();
      });
      it('should not duplicate if title is only whitespace', async () => {
        await render(data);

        fireEvent.input(screen.getByLabelText('Title:'), {
          target: { value: ' ' },
        });
        fireEvent.click(screen.getByText('Create'));

        expect(duplicateResumeSpy).not.toHaveBeenCalled();
        expect(updateResumeSpy).not.toHaveBeenCalled();
      });
      it('should not duplicate if the title is the same as the original', async () => {
        await render(data);

        fireEvent.input(screen.getByLabelText('Title:'), {
          target: { value: 'title text' },
        });
        fireEvent.click(screen.getByText('Create'));

        expect(duplicateResumeSpy).not.toHaveBeenCalled();
        expect(updateResumeSpy).not.toHaveBeenCalled();
      });
    });

    describe('Save Button', () => {
      it('should enable the save button if title is valid', async () => {
        await render(data);

        fireEvent.input(screen.getByLabelText('Title:'), {
          target: { value: 'new title' },
        });
        expect(
          screen.getByTestId('create').getAttribute('ng-reflect-disabled'),
        ).toBe('false');
      });
      it('should disable the save button by default', async () => {
        await render(data);

        expect(
          screen.getByTestId('create').getAttribute('ng-reflect-disabled'),
        ).toBe('true');
      });
      it('should disable the save button if the title is the same as original', async () => {
        await render(data);

        fireEvent.input(screen.getByLabelText('Title:'), {
          target: { value: 'title text' },
        });
        expect(
          screen.getByTestId('create').getAttribute('ng-reflect-disabled'),
        ).toBe('true');
      });
      it('should disable the save button if the title is empty', async () => {
        await render(data);

        fireEvent.input(screen.getByLabelText('Title:'), {
          target: { value: '' },
        });
        expect(
          screen.getByTestId('create').getAttribute('ng-reflect-disabled'),
        ).toBe('true');
      });
      it('should disable the save button if the title is only whitespace', async () => {
        await render(data);

        fireEvent.input(screen.getByLabelText('Title:'), {
          target: { value: ' ' },
        });
        expect(
          screen.getByTestId('create').getAttribute('ng-reflect-disabled'),
        ).toBe('true');
      });
    });

    describe('Errors', () => {
      it('should show error if the title is the same as the original', async () => {
        await render(data);

        fireEvent.input(screen.getByLabelText('Title:'), {
          target: { value: 'title text' },
        });
        fireEvent.click(screen.getByText('Create'));

        expect(screen.getByText('Title must be different')).toBeInTheDocument();
      });
      it('should show error if the title is empty', async () => {
        await render(data);

        fireEvent.input(screen.getByLabelText('Title:'), {
          target: { value: '' },
        });
        fireEvent.click(screen.getByText('Create'));

        expect(screen.getByText('Title cannot be blank')).toBeInTheDocument();
      });
      it('should show error if the title is only whitespace', async () => {
        await render(data);

        fireEvent.input(screen.getByLabelText('Title:'), {
          target: { value: ' ' },
        });
        fireEvent.click(screen.getByText('Create'));

        expect(screen.getByText('Title cannot be blank')).toBeInTheDocument();
      });
    });
  });
});

const render = async data => {
  return await renderRootComponent(UpdateTitleComponent, {
    routes: [
      {
        path: 'edit/:id',
        component: EditResumeComponent,
      },
    ],
    providers: [
      {
        provide: MatDialogRef,
        useClass: MockDialogRef,
      },
      {
        provide: MAT_DIALOG_DATA,
        useValue: data,
      },
    ],
  });
};

class MockDialogRef {
  close() {}
}
