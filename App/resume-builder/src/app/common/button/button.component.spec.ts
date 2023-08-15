import {fireEvent, screen} from '@testing-library/angular';
import { ButtonComponent } from './button.component';
import {renderRootComponent} from "../RenderRootComponent";
import {Router} from "@angular/router";

describe('ButtonComponent', () => {
  let routerSpy: jest.SpyInstance;
  beforeEach(() => {
    routerSpy = jest.spyOn(Router.prototype, "navigate");
  });

  it('should display text', async () => {
    await render('/','some text');
    expect(screen.getByText('some text')).toBeTruthy();
  });

  it('should have correct href', async () => {
    await render('/some/url','some text');
    const button = screen.getByText('some text');
    fireEvent.click(button);

    expect(routerSpy).toHaveBeenCalledTimes(1);
    expect(routerSpy).toHaveBeenCalledWith(['/some/url']);
  });

});


async function render(href:string,text:string){
  return await renderRootComponent(ButtonComponent, {
    componentProperties: {
      href: href,
      text: text,
    }
  })
}
