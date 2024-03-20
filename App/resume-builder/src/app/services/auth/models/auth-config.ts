import { AuthConfig } from 'angular-oauth2-oidc';

export const authConfig: AuthConfig = {
  issuer: 'https://auth.barnes7619.com/realms/ResumeBuilder/',
  redirectUri: 'https://localhost:4200/login-callback',
  clientId: 'resume-builder',
  scope: 'profile email roles resume-id',
  clearHashAfterLogin: false,
  strictDiscoveryDocumentValidation: false,
  skipIssuerCheck: true,
};
