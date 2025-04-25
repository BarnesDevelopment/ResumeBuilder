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

  it('should login', done => {
    service.login().subscribe({
      next: data => {
        expect(data).toBeNull();
        done();
      },
      error: () => fail('should not throw error'),
    });
    const req = httpMock.expectOne(`${baseUrl}/demo/login`);
    expect(req.request.method).toBe('PUT');
    req.flush(null, { status: 200, statusText: 'OK' });
  });

  it('should logout', done => {
    service.logout().subscribe({
      next: data => {
        expect(data).toBeNull();
        done();
      },
      error: () => fail('should not throw error'),
    });
    const req = httpMock.expectOne(`${baseUrl}/demo/logout`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null, { status: 200, statusText: 'OK' });
  });
});
