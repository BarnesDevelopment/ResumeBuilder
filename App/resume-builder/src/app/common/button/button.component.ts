import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { coerceBooleanProperty } from '@angular/cdk/coercion';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-button',
  templateUrl: './button.component.html',
  styleUrls: ['./button.component.scss'],
  standalone: true,
  imports: [NgClass],
})
export class ButtonComponent {
  @Input() href: string = '';
  @Input() buttonStyle: ButtonStyle = ButtonStyle.Primary;
  @Input() borderStyle: BorderStyle = BorderStyle.Dark;
  @Input() queryParams: any = {};
  private _disabled: boolean = false;
  @Input() get disabled() {
    return this._disabled;
  }

  set disabled(value: any) {
    this._disabled = coerceBooleanProperty(value);
  }

  private _ignoreClick: boolean = false;
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
