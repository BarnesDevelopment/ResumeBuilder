import { AuthService } from './auth.service';

describe('AuthServiceService', () => {
  let service: AuthService,
    oauthServiceSpy,
    demoServiceSpy,
    routerSpy,
    cookieServiceSpy;

  beforeEach(() => {
    oauthServiceSpy = {
      getIdentityClaims: jest.fn(),
      initCodeFlow: jest.fn(),
      logout: jest.fn(),
    };
    demoServiceSpy = {
      login: jest.fn(),
      logout: jest.fn(),
    };
    routerSpy = {
      navigate: jest.fn(),
    };
    cookieServiceSpy = {
      get: jest.fn(),
      delete: jest.fn(),
    };
    service = new AuthService(
      oauthServiceSpy,
      demoServiceSpy,
      routerSpy,
      cookieServiceSpy,
    );
  });

  describe('Demo', () => {
    it('should check for login cookie', () => {
      cookieServiceSpy.get.mockReturnValue('cookie');
      cookieServiceSpy.get.mockReturnValue('a good cookie');
      const loggedIn = service.isLoggedIn();
      expect(cookieServiceSpy.get).toHaveBeenCalledWith('resume-id');
      expect(service.loggedIn).toBe(true);
      expect(service.demo).toBe(true);
      expect(loggedIn).toBe(true);
    });
    it('should get claims', () => {
      service.loggedIn = true;
      service.demo = true;
      const claims = service.getClaims();
      expect(claims).toEqual({ name: 'Demo User' });
    });
  });
  describe('Oauth', () => {
    it('should check for login token', () => {});
    it('should get claims', () => {});
  });
});
