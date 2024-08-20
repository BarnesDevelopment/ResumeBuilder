import { TestBed } from '@angular/core/testing';

import { AuthInterceptor } from './auth.interceptor';
import { OAuthService } from 'angular-oauth2-oidc';
import {
  HttpEvent,
  HttpHandler,
  HttpRequest,
  HttpResponse,
} from '@angular/common/http';
import { Observable, of } from 'rxjs';
import mock = jest.mock;

describe('AuthInterceptor', () => {
  let request: HttpRequest<any> = new HttpRequest('GET', 'My.Domain.net');
  let handler;
  const mockHandler = {
    handle: (req: HttpRequest<any>): Observable<HttpEvent<any>> =>
      of(new HttpResponse(req)),
  };
  beforeEach(() => {
    handler = jest.spyOn(mockHandler, 'handle');
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
    handler.mockImplementation(() => of({}));
    const interceptor: AuthInterceptor = TestBed.inject(AuthInterceptor);
    interceptor.intercept(request, mockHandler);

    expect(handler).toHaveBeenCalledTimes(1);

    const lastCall = handler.mock.calls[0][0];
    expect(
      lastCall.headers.lazyUpdate.filter(x => x.name == 'Authorization')[0]
        .value,
    ).toEqual('Bearer MyAccessToken');
    expect(lastCall.url).toEqual('My.Domain.net');
  });
});
