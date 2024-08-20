import { HeaderComponent } from './header.component';
import { User } from '../../models/User';
import { within } from '@testing-library/angular';
import { Router } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';
import { renderRootComponent, fireEvent, screen } from '../testing-imports';

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
  beforeEach(() => {
    router = jest.spyOn(Router.prototype, 'navigate');
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
    it('should display login button if user is null', async () => {
      await render();
      const button = screen.getByTitle('login');
      expect(button).toBeTruthy();
    });

    it('should call oauth login method', async () => {
      const spy = jest.spyOn(MockOAuthService.prototype, 'initCodeFlow');
      await render();
      const button = screen.getByTitle('login');
      fireEvent.click(button);
      expect(spy).toHaveBeenCalledTimes(1);
    });

    it('should have correct login button text', async () => {
      await render();
      const button = screen.getByTitle('login');
      expect(within(button).queryByText('Login')).toBeInTheDocument();
    });
  });

  describe('User', () => {
    beforeEach(() => {
      jest
        .spyOn(MockOAuthService.prototype, 'getIdentityClaims')
        .mockReturnValue({ name: 'full name' });
      jest
        .spyOn(MockOAuthService.prototype, 'getIdToken')
        .mockReturnValue('fakeIdToken');
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

        it('should call oauth logout when logout button clicked', async () => {
          const spy = jest.spyOn(MockOAuthService.prototype, 'logOut');
          await render();
          const userButton = screen.getByText('full name');
          fireEvent.click(userButton);
          fireEvent.click(screen.getByText('Logout'));

          expect(spy).toHaveBeenCalledTimes(1);
        });
      });
    });
  });
});

const render = async () => {
  return await renderRootComponent(HeaderComponent, {
    providers: [
      {
        provide: OAuthService,
        useClass: MockOAuthService,
      },
    ],
  });
};

class MockOAuthService {
  getIdentityClaims() {
    return null;
  }

  getIdToken() {
    return null;
  }

  initCodeFlow() {}

  logOut() {}
}
