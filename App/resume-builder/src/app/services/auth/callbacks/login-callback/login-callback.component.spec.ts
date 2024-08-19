import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoginCallbackComponent } from './login-callback.component';
import { renderRootComponent } from '../../../../common/testing-imports';
import { Router } from '@angular/router';

describe('LoginCallbackComponent', () => {
  let router;
  beforeEach(async () => {
    router = jest.spyOn(Router.prototype, 'navigate');
    await renderRootComponent(LoginCallbackComponent, {});
  });

  it('should navigate', () => {
    expect(router).toHaveBeenCalledWith(['/'], { replaceUrl: true });
  });
});
