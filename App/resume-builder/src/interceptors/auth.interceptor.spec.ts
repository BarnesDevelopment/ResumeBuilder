import { TestBed } from '@angular/core/testing';

import { AuthInterceptor } from './auth.interceptor';
import { OAuthService } from 'angular-oauth2-oidc';
import { HttpEvent, HttpRequest, HttpResponse } from '@angular/common/http';
import { Observable, of } from 'rxjs';

describe('AuthInterceptor', () => {
  const request: HttpRequest<any> = new HttpRequest('GET', 'My.Domain.net');
  let handler, interceptor: AuthInterceptor;
  const mockHandler = {
    handle: (req: HttpRequest<any>): Observable<HttpEvent<any>> =>
      of(new HttpResponse(req)),
  };
  beforeEach(() => {
    handler = jest
      .spyOn(mockHandler, 'handle')
      .mockImplementation(() => of({} as HttpEvent<any>));

    return TestBed.configureTestingModule({
      providers: [
        AuthInterceptor,
        {
          provide: OAuthService,
          useValue: {
            getAccessToken: (): string => 'MyAccessToken',
          },
        },
      ],
    });
  });

  it('should add auth header', () => {
    interceptor = TestBed.inject(AuthInterceptor);
    interceptor.intercept(request, mockHandler);

    expect(handler).toHaveBeenCalledTimes(1);

    const lastCall = handler.mock.calls[0][0];
    expect(
      lastCall.headers.lazyUpdate.filter(x => x.name == 'Authorization')[0]
        .value,
    ).toEqual('Bearer MyAccessToken');
    expect(lastCall.url).toEqual('My.Domain.net');
  });

  it('should not add auth header', () => {
    TestBed.overrideProvider(OAuthService, {
      useValue: {
        getAccessToken: (): string => '',
      },
    });

    interceptor = TestBed.inject(AuthInterceptor);

    interceptor.intercept(request, mockHandler);

    expect(handler).toHaveBeenCalledTimes(1);

    const lastCall = handler.mock.calls[0][0];
    expect(lastCall.headers.lazyUpdate).toBeNull();
    expect(lastCall.url).toEqual('My.Domain.net');
  });
});
