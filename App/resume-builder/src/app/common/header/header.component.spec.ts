import { HeaderComponent } from './header.component';
import {renderRootComponent} from "../RenderRootComponent";
import {User} from "../../models/User";
import {fireEvent, screen} from "@testing-library/angular";

describe('HeaderComponent', () => {

  const user: User = {
    fullName: 'full name',
    email: '',
    id: '',
    firstName: 'full',
    lastName: 'name',
    username: ''
  };

  it('should display username if user is not null', async () => {
    await render(user);
    expect(screen.getByText('full name')).toBeTruthy();
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
      expect(button.getAttribute("href")).toBe('/login');
    });

    it('should have correct login button text', async () => {
      await render();
      const button = screen.getByTitle('login');
      expect(button.getAttribute("text")).toBe('Login');
    });
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
  });
});

const render = async (user: User = null) => {
  return await renderRootComponent(HeaderComponent, {
    componentProperties: {
      user: user
    }
  })
}
