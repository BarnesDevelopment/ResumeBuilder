import { HomeComponent } from './home.component';
import { renderRootComponent, screen, Guid } from '../common/testing-imports';
import { ResumeHeader } from '../models/Resume';
import { OAuthService } from 'angular-oauth2-oidc';
import { ResumeService } from '../resume/services/resume.service';
import { of } from 'rxjs';

describe('HomeComponent', () => {
  beforeEach(() => {
    jest.resetAllMocks();
  });
  describe('Logged In', () => {
    let resumeServiceSpy;
    beforeEach(() => {
      resumeServiceSpy = jest
        .spyOn(ResumeService.prototype, 'getResumes')
        .mockReturnValue(of([]));
      jest
        .spyOn(MockOauthService.prototype, 'getIdToken')
        .mockReturnValue('fakeIdToken');
    });
    it('should show create resume card', async () => {
      await render();
      expect(screen.getByText('Create Resume')).toBeTruthy();
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

const render = async () => {
  return renderRootComponent(HomeComponent, {
    providers: [
      {
        provide: OAuthService,
        useClass: MockOauthService,
      },
    ],
  });
};

class MockOauthService {
  getIdToken(): string {
    return null;
  }
}
