import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoginSplashScreenComponent } from './login-splash-screen.component';

describe('LoginSplashScreenComponent', () => {
  let component: LoginSplashScreenComponent;
  let fixture: ComponentFixture<LoginSplashScreenComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [LoginSplashScreenComponent]
    });
    fixture = TestBed.createComponent(LoginSplashScreenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
