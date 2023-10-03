import { screen } from '@testing-library/angular';

import { InputComponent } from './input.component';
import { renderRootComponent } from '../RenderRootComponent';
import {
  FormsModule,
  NG_VALUE_ACCESSOR,
  ReactiveFormsModule,
} from '@angular/forms';
import { forwardRef } from '@angular/core';

describe('InputComponent', () => {
  it('should show label', async () => {
    await render('Test');
    expect(screen.getByText('Test:')).toBeTruthy();
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
});

const render = async (title: string) => {
  return await renderRootComponent(InputComponent, {
    componentProperties: {
      title,
    },
    imports: [FormsModule, ReactiveFormsModule],
    providers: [
      {
        provide: NG_VALUE_ACCESSOR,
        useExisting: forwardRef(() => InputComponent),
        multi: true,
      },
    ],
  });
};

const renderType = async (title: string, type: string) => {
  return await renderRootComponent(InputComponent, {
    componentProperties: {
      title,
      type,
    },
    imports: [FormsModule, ReactiveFormsModule],
    providers: [
      {
        provide: NG_VALUE_ACCESSOR,
        useExisting: forwardRef(() => InputComponent),
        multi: true,
      },
    ],
  });
};
