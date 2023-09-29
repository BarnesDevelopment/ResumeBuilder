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
  @Input() buttonStyle: ButtonStyle = ButtonStyle.Primary;
  @Input() borderStyle: BorderStyle = BorderStyle.Dark;
  @Input() queryParams: any = {};

  constructor(private router: Router) {}

  GoTo(): void {
    this.router.navigate([this.href], {
      queryParams: this.queryParams,
    });
  }
}

export enum ButtonStyle {
  Primary = 'primary',
  Secondary = 'secondary',
  Alert = 'alert',
  Transparent = 'transparent',
}

export enum BorderStyle {
  Light = 'light',
  Dark = 'dark',
}
