import { LoginSplashScreenComponent } from './login-splash-screen.component';
import { fireEvent, renderRootComponent, screen } from '../testing-imports';
import { OAuthService } from 'angular-oauth2-oidc';

describe('LoginSplashScreenComponent', () => {
  let spy;
  beforeEach(async () => {
    spy = jest.spyOn(MockOAuthService.prototype, 'initCodeFlow');
    await renderRootComponent(LoginSplashScreenComponent, {
      providers: [{ provide: OAuthService, useClass: MockOAuthService }],
    });
  });

  it('should create', () => {
    fireEvent.click(screen.getByText('Login'));

    expect(spy).toHaveBeenCalledTimes(1);
  });
});

class MockOAuthService {
  initCodeFlow() {}
}
