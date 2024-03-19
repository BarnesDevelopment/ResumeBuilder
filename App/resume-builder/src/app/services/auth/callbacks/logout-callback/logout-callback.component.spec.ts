import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LogoutCallbackComponent } from './logout-callback.component';

describe('LogoutCallbackComponent', () => {
  let component: LogoutCallbackComponent;
  let fixture: ComponentFixture<LogoutCallbackComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [LogoutCallbackComponent]
    });
    fixture = TestBed.createComponent(LogoutCallbackComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
