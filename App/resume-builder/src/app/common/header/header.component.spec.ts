import { HeaderComponent } from './header.component';
import {renderRootComponent} from "../RenderRootComponent";
import {User} from "../../models/User";
import {screen} from "@testing-library/angular";

describe('HeaderComponent', () => {

  const user: User = {
    fullName: 'full name',
    email: '',
    id: '',
    firstName: 'full',
    lastName: 'name',
    username: ''
  };

  beforeEach(() => {

  });

  it('should display username if user is not null', async () => {
    await render(user);
    expect(screen.getByText('full name')).toBeTruthy();
  });

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

const render = async (user: User = null) => {
  return await renderRootComponent(HeaderComponent, {
    componentProperties: {
      user: user
    }
  })
}
