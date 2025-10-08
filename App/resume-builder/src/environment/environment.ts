import { Environment } from './environment-model';

export const environment: Environment = {
  production: false,
  demo: false,
  apiBasePath: 'https://localhost:7211/resume',
  domain: 'localhost:4200',
  fusionAuthRedirectUri: 'http://localhost:4200/login/callback',
  loggedIn: false,
};
