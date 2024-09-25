import { HomeComponent } from './home.component';
import { Guid, renderRootComponent, screen } from '../common/testing-imports';
import { OAuthService } from 'angular-oauth2-oidc';
import { ResumeService } from '../resume/services/resume.service';
import { of } from 'rxjs';
import { fireEvent } from '@testing-library/angular';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { UpdateTitleComponent } from '../resume/components/update-title/update-title.component';

describe('HomeComponent', () => {
  beforeEach(() => {
    jest.resetAllMocks();
  });
  describe('Logged In', () => {
    let resumeServiceSpy, deleteSpy;
    beforeEach(() => {
      resumeServiceSpy = jest
        .spyOn(ResumeService.prototype, 'getResumes')
        .mockReturnValue(of([]));
      deleteSpy = jest
        .spyOn(ResumeService.prototype, 'deleteNode')
        .mockReturnValue(of(true));
      jest
        .spyOn(MockOauthService.prototype, 'getIdToken')
        .mockReturnValue('fakeIdToken');
    });
    it('should show create resume card', async () => {
      await render();
      expect(screen.getByText('Create Resume')).toBeTruthy();
    });
    it('should duplicate resume', async () => {
      const id = Guid.create();
      resumeServiceSpy.mockReturnValue(
        of([
          {
            content: 'Resume 1',
            comments: 'This is my first resume',
            id: id,
          },
        ]),
      );
      const dialogSpy = jest.spyOn(MockDialog.prototype, 'open');
      await render();
      fireEvent.click(screen.getByText('Copy'));

      expect(dialogSpy).toHaveBeenCalledWith(UpdateTitleComponent, {
        data: { id: id, next: 1 },
        disableClose: true,
        width: '25rem',
        height: '15rem',
      });
    });
    it('should delete resume', async () => {
      resumeServiceSpy.mockReturnValue(
        of([
          {
            content: 'Resume 1',
            comments: 'This is my first resume',
            id: Guid.create(),
          },
          {
            content: 'Resume 2',
            comments: 'This is my second resume',
            id: Guid.create(),
          },
          {
            content: 'Resume 3',
            comments: 'This is my third resume',
            id: Guid.create(),
          },
        ]),
      );
      await render();

      fireEvent.click(screen.getAllByText('Delete')[0]);

      expect(screen.queryByText('Resume 1')).toBeFalsy();
      expect(deleteSpy).toHaveBeenCalled();
    });
    it('should show resume cards', async () => {
      resumeServiceSpy.mockReturnValue(
        of([
          {
            content: 'Resume 1',
            comments: 'This is my first resume',
            id: Guid.create(),
          },
          {
            content: 'Resume 2',
            comments: 'This is my second resume',
            id: Guid.create(),
          },
          {
            content: 'Resume 3',
            comments: 'This is my third resume',
            id: Guid.create(),
          },
        ]),
      );
      await render();
      expect(screen.getByText('Resume 1')).toBeTruthy();
      expect(screen.getByText('This is my first resume')).toBeTruthy();
      expect(screen.getByText('Resume 2')).toBeTruthy();
      expect(screen.getByText('This is my second resume')).toBeTruthy();
      expect(screen.getByText('Resume 3')).toBeTruthy();
      expect(screen.getByText('This is my third resume')).toBeTruthy();
    });
  });
  describe('Logged Out', () => {
    it('should show dumb cards if login is false', async () => {
      await render();
      expect(screen.getByTitle('dumb cards')).toBeTruthy();
    });

    it('should show login splash screen if login is false', async () => {
      await render();
      expect(screen.getByTitle('login splash')).toBeTruthy();
    });
  });
});

class MockOauthService {
  getIdToken(): string {
    return null;
  }
}

class MockDialog {
  open() {}
}

const render = async () => {
  return renderRootComponent(HomeComponent, {
    imports: [MatDialogModule],
    providers: [
      {
        provide: OAuthService,
        useClass: MockOauthService,
      },
      {
        provide: MatDialog,
        useClass: MockDialog,
      },
    ],
  });
};
