import { TestBed } from '@angular/core/testing';

import { DemoService } from './demo.service';
import {
  HttpClientTestingModule,
  HttpTestingController,
} from '@angular/common/http/testing';
import { ResumeService } from '../../resume/services/resume.service';

describe('DemoService', () => {
  let service: DemoService;
  let httpMock: HttpTestingController;
  let baseUrl: string;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ResumeService],
    });
    service = TestBed.inject(DemoService);
    httpMock = TestBed.inject(HttpTestingController);

    baseUrl = service.env.apiBasePath;
  });
  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
