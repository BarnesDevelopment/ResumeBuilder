import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-button',
  templateUrl: './button.component.html',
  styleUrls: ['./button.component.scss'],
})
export class ButtonComponent {
  @Input() href: string = '';
  @Input() text: string = '';
  @Input() style: ButtonStyle = ButtonStyle.Primary;

  constructor(private router: Router) {}

  GoTo(): void {
    this.router.navigate([this.href]);
  }
}

export enum ButtonStyle {
  Primary = 'primary',
  Secondary = 'secondary',
  Alert = 'alert',
  Transparent = 'transparent',
}
