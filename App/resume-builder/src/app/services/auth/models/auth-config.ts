import { AuthConfig } from 'angular-oauth2-oidc';
import { environment } from '../../../../environment/environment';

export const authConfig: AuthConfig = {
  issuer: 'https://auth.barnes7619.com/realms/ResumeBuilder',
  redirectUri: `https://${environment.domain}/login-callback`,
  postLogoutRedirectUri: `https://${environment.domain}/`,
  silentRefreshRedirectUri: `https://${environment.domain}/silent-callback`,
  clientId: 'resume-builder',
  dummyClientSecret: 'uh01jhYZqvotfl2I9DbNkTEm0QLBNKEH',
  scope: 'profile offline_access',
  responseType: 'code',
  showDebugInformation: true,
  strictDiscoveryDocumentValidation: false,
};
