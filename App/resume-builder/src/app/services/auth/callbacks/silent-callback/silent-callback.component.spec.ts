import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SilentCallbackComponent } from './silent-callback.component';

describe('SilentCallbackComponent', () => {
  let component: SilentCallbackComponent;
  let fixture: ComponentFixture<SilentCallbackComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [SilentCallbackComponent],
    });
    fixture = TestBed.createComponent(SilentCallbackComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
