import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SectionWorkExperienceComponent } from './section-work-experience.component';

describe('SectionWorkExperienceComponent', () => {
  let component: SectionWorkExperienceComponent;
  let fixture: ComponentFixture<SectionWorkExperienceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SectionWorkExperienceComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SectionWorkExperienceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
