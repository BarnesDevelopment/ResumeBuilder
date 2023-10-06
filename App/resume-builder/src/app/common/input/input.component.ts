import {
  AfterContentInit,
  Component,
  ContentChild,
  ElementRef,
  Input,
  Optional,
  Self,
  ViewChild,
} from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';

@Component({
  selector: 'app-input',
  templateUrl: './input.component.html',
  styleUrls: ['./input.component.scss'],
})
export class InputComponent implements ControlValueAccessor {
  @Input() type: string = 'text';
  @Input() errorMessage: string;
  @Input() formControlName: string;

  value: string;
  disabled = false;

  constructor(@Self() @Optional() private control: NgControl) {
    if (control) {
      control.valueAccessor = this;
    }
  }

  public get invalid(): boolean {
    return this.control ? this.control.invalid && this.control.touched : false;
  }

  public get showError(): boolean {
    if (!this.control) {
      return false;
    }

    const { dirty, touched } = this.control;

    return this.invalid ? dirty || touched : false;
  }
  onChange: any = (value: any) => {};
  onTouched: any = () => {};

  writeValue(obj: any): void {
    this.value = obj;
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }
}