import { NgModule } from '@angular/core';
import { HeaderComponent } from './header/header.component';
import { NgIf } from '@angular/common';
import { ButtonComponent } from './button/button.component';

@NgModule({
  declarations: [HeaderComponent, ButtonComponent],
  exports: [HeaderComponent, ButtonComponent],
  imports: [NgIf],
})
export class CommonModule {}
