import { screen } from '@testing-library/angular';
import { InputComponent } from './input.component';
import { renderRootComponent } from '../RenderRootComponent';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Component, Input } from '@angular/core';

describe('InputComponent', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  it('should show label', async () => {
    await renderContainer('Test');

    expect(screen.getByText('Test:')).toBeTruthy();
  });

  it('should have correct title', async () => {
    await render('Test');
    expect(screen.getByTitle('Test')).toBeTruthy();
  });

  it('should be of correct input type by default', async () => {
    await render('Test');
    const input = screen.getByTitle('Test');
    expect(input.getAttribute('type')).toBe('text');
  });

  it('should be of correct input type when given', async () => {
    await renderType('Test', 'number');
    const input = screen.getByTitle('Test');
    expect(input.getAttribute('type')).toBe('number');
  });

  it('should show error message when invalid', async () => {
    jest
      .spyOn(InputComponent.prototype, 'invalid', 'get')
      .mockReturnValue(true);
    await render('Test');
    expect(screen.getByText('This field is required!')).toBeTruthy();
  });

  it('should show custom error message', async () => {
    jest
      .spyOn(InputComponent.prototype, 'invalid', 'get')
      .mockReturnValue(true);
    await renderType('Test', 'text', 'Custom error message');
    expect(screen.getByText('Custom error message')).toBeTruthy();
  });

  it('should not show error message when valid', async () => {
    jest
      .spyOn(InputComponent.prototype, 'invalid', 'get')
      .mockReturnValue(false);
    await render('Test');
    expect(screen.queryByText('This field is required!')).toBeFalsy();
  });

  const render = async (title: string) => {
    return await renderRootComponent(InputComponent, {
      componentProperties: {
        formControlName: title,
      },
      imports: [FormsModule, ReactiveFormsModule],
    });
  };

  const renderContainer = async (title: string) => {
    return await renderRootComponent(InputContainerComponent, {
      componentProperties: {
        title,
      },
      imports: [FormsModule, ReactiveFormsModule],
    });
  };

  const renderType = async (
    title: string,
    type: string,
    errorMessage: string = null,
  ) => {
    return await renderRootComponent(InputComponent, {
      componentProperties: {
        formControlName: title,
        type,
        errorMessage,
      },
      imports: [FormsModule, ReactiveFormsModule],
    });
  };
});

@Component({
  standalone: true,
  selector: 'app-input-container',
  template:
    '<app-input formControlName="formControlName">{{title}}</app-input>',
  imports: [InputComponent],
})
class InputContainerComponent {
  @Input() title: string;
}
