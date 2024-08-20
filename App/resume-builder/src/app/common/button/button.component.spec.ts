import { fireEvent, screen } from '@testing-library/angular';
import { ButtonComponent } from './button.component';
import { renderRootComponent } from '../RenderRootComponent';
import { Router } from '@angular/router';
import { Component, Input } from '@angular/core';
import { InputComponent } from '../input/input.component';
import { NgIf } from '@angular/common';

describe('ButtonComponent', () => {
  let routerSpy: jest.SpyInstance;
  beforeEach(() => {
    routerSpy = jest.spyOn(Router.prototype, 'navigate');
  });

  it('should display text', async () => {
    await renderContainer('some text');
    expect(screen.getByText('some text')).toBeTruthy();
  });

  it('should navigate with correct href', async () => {
    await render('/some/url');
    const button = screen.getByTestId('button');
    fireEvent.click(button);

    expect(routerSpy).toHaveBeenCalledTimes(1);
    expect(routerSpy).toHaveBeenCalledWith(['/some/url'], {
      queryParams: {},
    });
  });

  it('should navigate with correct queryParams', async () => {
    await render('/some/url', { key: 'value' });
    const button = screen.getByTestId('button');
    fireEvent.click(button);

    expect(routerSpy).toHaveBeenCalledTimes(1);
    expect(routerSpy).toHaveBeenCalledWith(['/some/url'], {
      queryParams: { key: 'value' },
    });
  });

  it('should not navigate when ignoreClick is true', async () => {
    await renderContainer('title', true);
    const button = screen.getByTestId('button');
    fireEvent.click(button);

    expect(routerSpy).not.toHaveBeenCalled();
  });
});

async function render(href: string, queryParams = {}) {
  return await renderRootComponent(ButtonComponent, {
    componentInputs: {
      href,
      queryParams,
    },
    routes: [
      {
        path: 'some/url',
        component: InputComponent,
      },
    ],
  });
}

async function renderContainer(title: string, ignoreClick = false) {
  return await renderRootComponent(ButtonContainerComponent, {
    componentInputs: {
      title,
      ignoreClick,
    },
  });
}

@Component({
  standalone: true,
  selector: 'app-button-container',
  template:
    '<app-button ignoreClick *ngIf="ignoreClick">{{title}}</app-button><app-button *ngIf="!ignoreClick">{{title}}</app-button>',
  imports: [ButtonComponent, NgIf],
})
class ButtonContainerComponent {
  @Input() title: string;
  @Input() ignoreClick = false;
}
