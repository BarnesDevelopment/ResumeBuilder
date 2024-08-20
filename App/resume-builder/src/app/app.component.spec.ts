import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { AppComponent } from './app.component';
import { renderRootComponent } from './common/testing-imports';
import { OAuthService } from 'angular-oauth2-oidc';
import { authConfig } from './services/auth/models/auth-config';

describe('AppComponent', () => {
  let configure, setup, load;
  beforeEach(async () => {
    configure = jest.spyOn(MockOAuthService.prototype, 'configure');
    setup = jest.spyOn(
      MockOAuthService.prototype,
      'setupAutomaticSilentRefresh',
    );
    load = jest
      .spyOn(MockOAuthService.prototype, 'loadDiscoveryDocumentAndTryLogin')
      .mockReturnValue(Promise.resolve());
    await renderRootComponent(AppComponent, {
      providers: [{ provide: OAuthService, useClass: MockOAuthService }],
    });
  });

  it('should setup oauth', () => {
    expect(configure).toBeCalledWith({
      issuer: 'https://auth.barnes7619.com/realms/ResumeBuilder',
      redirectUri: 'https://localhost:4200/login-callback',
      postLogoutRedirectUri: 'https://localhost:4200/',
      silentRefreshRedirectUri: 'https://localhost:4200/silent-callback',
      clientId: 'resume-builder',
      dummyClientSecret: 'uh01jhYZqvotfl2I9DbNkTEm0QLBNKEH',
      scope: 'profile offline_access',
      responseType: 'code',
      showDebugInformation: true,
      strictDiscoveryDocumentValidation: false,
    });
    expect(setup).toBeCalledTimes(1);
    expect(load).toBeCalledTimes(1);
  });
});

class MockOAuthService {
  configure() {}

  setupAutomaticSilentRefresh() {}

  loadDiscoveryDocumentAndTryLogin(): Promise<any> {
    return Promise.resolve();
  }
}
