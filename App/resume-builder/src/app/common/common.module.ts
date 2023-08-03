import { NgModule } from '@angular/core';
import { HeaderComponent } from './header/header.component';
import { NgIf } from '@angular/common';
import { ButtonComponent } from './button/button.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

@NgModule({
  declarations: [HeaderComponent, ButtonComponent],
  exports: [HeaderComponent, ButtonComponent],
  imports: [NgIf, FontAwesomeModule],
})
export class CommonModule {}
