import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { coerceBooleanProperty } from '@angular/cdk/coercion';

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
  private _ignoreClick: boolean;
  @Input()
  get ignoreClick() {
    return this._ignoreClick;
  }
  set ignoreClick(value: any) {
    this._ignoreClick = coerceBooleanProperty(value);
  }
  @Input() onClick: any = () => {};

  constructor(private router: Router) {}

  GoTo(): void {
    if (!this._ignoreClick) {
      this.router.navigate([this.href], {
        queryParams: this.queryParams,
      });
    }
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
