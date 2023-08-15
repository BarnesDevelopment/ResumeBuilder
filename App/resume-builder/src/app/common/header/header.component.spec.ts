import { HeaderComponent } from './header.component';
import { renderRootComponent } from '../RenderRootComponent';
import { User } from '../../models/User';
import { fireEvent, screen } from '@testing-library/angular';
import { Router } from '@angular/router';

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

    it('should have correct login href', async () => {
      await render();
      const button = screen.getByTitle('login');
      expect(button.getAttribute('href')).toBe('/login');
    });

    it('should have correct login button text', async () => {
      await render();
      const button = screen.getByTitle('login');
      expect(button.getAttribute('text')).toBe('Login');
    });
  });

  describe('User', () => {
    it('should display username if user is not null', async () => {
      await render(user);
      expect(screen.getByText('full name')).toBeTruthy();
    });

    describe('User Panel', () => {
      it('should show user panel when toggle is clicked', async () => {
        await render(user);
        const userButton = screen.getByText('full name');
        fireEvent.click(userButton);
        expect(screen.getByTitle('userPanel')).toBeTruthy();
      });

      it('should hide user panel when toggle is clicked again', async () => {
        await render(user);
        const userButton = screen.getByText('full name');
        fireEvent.click(userButton);
        fireEvent.click(userButton);
        expect(screen.queryByTitle('userPanel')).toBeFalsy();
      });

      it('should hide user panel by default', async () => {
        await render(user);
        expect(screen.queryByTitle('userPanel')).toBeFalsy();
      });

      describe('Panel Rows', () => {
        it('should show account button', async () => {
          await render(user);
          const userButton = screen.getByText('full name');
          fireEvent.click(userButton);
          expect(screen.getByText('Account')).toBeTruthy();
        });

        it('should goto account page when account button is clicked', async () => {
          await render(user);
          const userButton = screen.getByText('full name');
          fireEvent.click(userButton);
          const button = screen.getByText('Account');
          fireEvent.click(button);
          expect(router).toHaveBeenCalledTimes(1);
          expect(router).toHaveBeenCalledWith(['/account']);
        });

        it('should show logout button', async () => {
          await render(user);
          const userButton = screen.getByText('full name');
          fireEvent.click(userButton);
          expect(screen.getByText('Logout')).toBeTruthy();
        });
      });
    });
  });
});

const render = async (user: User = null) => {
  return await renderRootComponent(HeaderComponent, {
    componentProperties: {
      user: user,
    },
  });
};
