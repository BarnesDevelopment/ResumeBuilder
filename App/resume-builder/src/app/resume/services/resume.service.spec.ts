import { ResumeService } from './resume.service';
import {
  HttpClient,
  HttpEvent,
  HttpHandler,
  HttpRequest,
} from '@angular/common/http';
import { firstValueFrom, Observable, of } from 'rxjs';
import { Guid, renderRootComponent } from '../../common/testing-imports';
import {
  HttpClientTestingModule,
  HttpTestingController,
} from '@angular/common/http/testing';
import { Type } from '@angular/core';

describe('ResumeService', () => {
  beforeEach(() => {});

  describe('Delete', () => {
    it('should call http delete with correct url', async () => {
      const guid = Guid.create();
      const { fixture } = await renderRootComponent(ResumeService, {
        imports: [HttpClientTestingModule],
      });
      const httpMock = fixture.debugElement.injector.get<HttpTestingController>(
        HttpTestingController as Type<HttpTestingController>,
      );

      fixture.componentInstance.deleteNode(Guid.create());
      const req = httpMock.expectOne(
        `${fixture.componentInstance.env.apiBasePath}/resume/delete/${guid}`,
      );

      req.flush(true);

      expect(req.request.method).toBe('DELETE');
    });
    it.each([202, 404])('should not throw error on %i', async () => {});
    it.each([400, 401, 403, 500])('should throw error on %i', async () => {});
  });
});

class FakeHttpHandler extends HttpHandler {
  handle(req: HttpRequest<any>): Observable<HttpEvent<any>> {
    throw new Error('Method not implemented.');
  }
}
