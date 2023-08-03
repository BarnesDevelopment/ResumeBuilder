import { NgModule } from '@angular/core';
import { LoginPageComponent } from './components/login-page/login-page.component';
import { CommonComponentsModule } from '../common/common-components.module';

@NgModule({
  declarations: [LoginPageComponent],
  imports: [CommonComponentsModule],
})
export class LoginModule {}
