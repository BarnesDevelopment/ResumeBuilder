import { ResumeService } from './resume.service';
import { throwError } from 'rxjs';
import { Guid } from 'guid-typescript';
import {
  HttpClientTestingModule,
  HttpTestingController,
} from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

describe('ResumeService', () => {
  let service: ResumeService;
  let httpMock: HttpTestingController;
  let baseUrl: string;
  const guid = Guid.create();

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ResumeService],
    });
    service = TestBed.inject(ResumeService);
    httpMock = TestBed.inject(HttpTestingController);

    baseUrl = service.env.apiBasePath;
  });
  afterEach(() => {
    httpMock.verify();
  });

  it('should duplicate resume', done => {
    service.duplicateResume(guid.toString()).subscribe({
      next: data => {
        expect(data).toBeTruthy();
        done();
      },
      error: () => fail('should not throw error'),
    });
    const req = httpMock.expectOne(`${baseUrl}/duplicate/${guid}`);
    expect(req.request.method).toBe('POST');
    req.flush(guid.toString(), { status: 200, statusText: 'OK' });
  });

  describe('Delete', () => {
    it('should call http delete with correct url', done => {
      service.deleteNode(guid).subscribe({
        next: data => {
          expect(data).toBeTruthy();
          done();
        },
        error: () => fail('should not throw error'),
      });
      const req = httpMock.expectOne(`${baseUrl}/delete/${guid}`);
      expect(req.request.method).toBe('DELETE');
      req.flush(true, { status: 202, statusText: 'Accepted' });
    });
    it.each([202, 404])(
      'should not throw error on %i',
      (statusCode: number, done) => {
        const statusText = statusCode == 202 ? 'Accepted' : 'Not Found';
        service.deleteNode(guid).subscribe({
          next: data => {
            expect(data).toBeTruthy();
            done();
          },
          error: () => {
            expect(true).toBeFalsy();
            done();
          },
        });
        const req = httpMock.expectOne(`${baseUrl}/delete/${guid}`);
        req.flush(true, { status: statusCode, statusText: statusText });
      },
    );
    it.each([400, 401, 403, 500])(
      'should throw error on %i',
      (statusCode: number, done) => {
        const statusText =
          statusCode == 400
            ? 'Bad Request'
            : statusCode == 401
            ? 'Unauthorized'
            : statusCode == 403
            ? 'Forbidden'
            : 'Internal Server Error';
        service.deleteNode(guid).subscribe({
          next: () => throwError(() => new Error('should not call next')),
          error: error => {
            expect(error.status).toBe(statusCode);
            done();
          },
        });
        const req = httpMock.expectOne(`${baseUrl}/delete/${guid}`);
        req.flush(null, { status: statusCode, statusText: statusText });
      },
    );
  });
});
