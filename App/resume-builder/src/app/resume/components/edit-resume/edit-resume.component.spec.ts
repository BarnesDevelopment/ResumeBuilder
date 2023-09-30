import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditResumeComponent } from './edit-resume.component';

describe('EditResumeComponent', () => {
  let component: EditResumeComponent;
  let fixture: ComponentFixture<EditResumeComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [EditResumeComponent]
    });
    fixture = TestBed.createComponent(EditResumeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
