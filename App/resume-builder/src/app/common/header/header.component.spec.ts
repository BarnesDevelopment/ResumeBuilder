import { HeaderComponent } from './header.component';
import { User } from '../../models/User';
import { within } from '@testing-library/angular';
import { Router } from '@angular/router';
import { fireEvent, renderRootComponent, screen } from '../testing-imports';
import { AuthService } from '../../services/auth/auth.service';
import { OAuthService } from 'angular-oauth2-oidc';

describe('HeaderComponent', () => {
  const user: User = {
    fullName: 'full name',
    email: '',
    id: '',
    firstName: 'full',
    lastName: 'name',
    username: '',
  };
  let router: jest.SpyInstance;
  let authServiceLoggedIn,
    authServiceClaims,
    authServiceLogin,
    authServiceLogout;
  beforeEach(() => {
    jest.resetAllMocks();
    router = jest.spyOn(Router.prototype, 'navigate');
    authServiceLoggedIn = jest
      .spyOn(AuthService.prototype, 'isLoggedIn')
      .mockReturnValue(true);
    authServiceClaims = jest
      .spyOn(AuthService.prototype, 'getClaims')
      .mockReturnValue(user);
    authServiceLogin = jest.spyOn(AuthService.prototype, 'login');
    authServiceLogout = jest.spyOn(AuthService.prototype, 'logout');
  });

  describe('Title', () => {
    it('should display title', async () => {
      await render();
      expect(screen.getByText('Barnes Resume Builder')).toBeTruthy();
    });

    it('should navigate to home if title is clicked', async () => {
      await render();
      const title = screen.getByText('Barnes Resume Builder');
      fireEvent.click(title);
      expect(router).toHaveBeenCalledTimes(1);
      expect(router).toHaveBeenCalledWith(['/']);
    });
  });

  describe('Login Button', () => {
    beforeEach(() => {
      authServiceLoggedIn.mockReturnValue(false);
    });
    it('should display login button if not logged in', async () => {
      await render();
      const button = screen.getByTitle('login');
      expect(button).toBeTruthy();
    });

    it('should call auth login method', async () => {
      await render();
      const button = screen.getByTitle('login');
      fireEvent.click(button);
      expect(authServiceLogin).toHaveBeenCalledTimes(1);
    });

    it('should have correct login button text', async () => {
      await render();
      const button = screen.getByTitle('login');
      expect(within(button).queryByText('Login')).toBeInTheDocument();
    });
  });

  describe('User', () => {
    beforeEach(() => {
      authServiceClaims.mockReturnValue({ name: 'full name' });
    });

    it('should display username if user is not null', async () => {
      await render();
      expect(screen.getByText('full name')).toBeTruthy();
    });

    describe('User Panel', () => {
      it('should show user panel when toggle is clicked', async () => {
        await render();
        const userButton = screen.getByText('full name');
        fireEvent.click(userButton);
        expect(screen.queryByTitle('userPanel')).toBeInTheDocument();
      });

      it('should hide user panel when toggle is clicked again', async () => {
        await render();
        const userButton = screen.getByText('full name');
        fireEvent.click(userButton);
        fireEvent.click(userButton);
        expect(screen.queryByTitle('userPanel')).toBeFalsy();
      });

      it('should hide user panel by default', async () => {
        await render();
        expect(screen.queryByTitle('userPanel')).toBeFalsy();
      });

      describe('Panel Rows', () => {
        it('should show account button', async () => {
          await render();
          const userButton = screen.getByText('full name');
          fireEvent.click(userButton);
          expect(screen.getByText('Account')).toBeTruthy();
        });

        it('should goto account page when account button is clicked', async () => {
          await render();
          const userButton = screen.getByText('full name');
          fireEvent.click(userButton);
          const button = screen.getByText('Account');
          fireEvent.click(button);
          expect(router).toHaveBeenCalledTimes(1);
          expect(router).toHaveBeenCalledWith(['/account']);
        });

        it('should call auth logout when logout button clicked', async () => {
          await render();
          const userButton = screen.getByText('full name');
          fireEvent.click(userButton);
          fireEvent.click(screen.getByText('Logout'));

          expect(authServiceLogout).toHaveBeenCalledTimes(1);
        });
      });
    });
  });
});

class MockOauthService {
  getIdToken(): string {
    return null;
  }
}

const render = async () => {
  return await renderRootComponent(HeaderComponent, {
    providers: [
      {
        provide: OAuthService,
        useClass: MockOauthService,
      },
    ],
  });
};
