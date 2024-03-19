import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginPageComponent } from './login/components/login-page/login-page.component';
import { HomeComponent } from './home/home.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { CreateResumeComponent } from './resume/components/create-resume/create-resume.component';
import { EditResumeComponent } from './resume/components/edit-resume/edit-resume.component';
import { LoginCallbackComponent } from './services/auth/callbacks/login-callback/login-callback.component';
import { LogoutCallbackComponent } from './services/auth/callbacks/logout-callback/logout-callback.component';
import { SilentCallbackComponent } from './services/auth/callbacks/silent-callback/silent-callback.component';
import { AuthGuard } from './guards/auth.guard';

const routes: Routes = [
  {
    path: '',
    component: HomeComponent,
  },
  {
    path: 'login',
    component: LoginPageComponent,
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
  },
  {
    path: 'create',
    component: CreateResumeComponent,
  },
  {
    path: 'edit/:id',
    component: EditResumeComponent,
  },
  { path: '**', component: PageNotFoundComponent },
  { path: 'unauthorized', component: PageNotFoundComponent },
  {
    path: 'login-callback',
    component: LoginCallbackComponent,
  },
  {
    path: 'logout-callback',
    component: LogoutCallbackComponent,
  },
  {
    path: 'silent-callback',
    component: SilentCallbackComponent,
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { onSameUrlNavigation: 'reload' })],
  exports: [RouterModule],
})
export class AppRoutingModule {}
