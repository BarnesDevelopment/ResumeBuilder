import { TestBed } from '@angular/core/testing';

import { CustomErrorHttpInterceptor } from './http-interceptor.interceptor';

describe('HttpInterceptorInterceptor', () => {
  beforeEach(() =>
    TestBed.configureTestingModule({
      providers: [CustomErrorHttpInterceptor],
    }),
  );

  it('should be created', () => {
    const interceptor: CustomErrorHttpInterceptor = TestBed.inject(
      CustomErrorHttpInterceptor,
    );
    expect(interceptor).toBeTruthy();
  });
});
