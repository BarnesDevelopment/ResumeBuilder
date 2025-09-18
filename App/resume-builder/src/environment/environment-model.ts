export interface Environment {
  production: boolean;
  demo: boolean;
  apiBasePath: string;
  domain: string;
  loggedIn: boolean;
  fusionAuthRedirectUri: string;
}
