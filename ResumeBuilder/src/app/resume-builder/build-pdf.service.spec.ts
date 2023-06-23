import { TestBed } from '@angular/core/testing';

import { BuildPdfService } from './build-pdf.service';

describe('BuildPdfService', () => {
  let service: BuildPdfService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BuildPdfService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
