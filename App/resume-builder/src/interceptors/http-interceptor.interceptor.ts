import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../environment/environment';

@Injectable()
export class CustomErrorHttpInterceptor implements HttpInterceptor {
  intercept(
    req: HttpRequest<any>,
    next: HttpHandler,
  ): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((err) => {
        console.log(environment.production);
        if (err instanceof HttpErrorResponse && !environment.production) {
          // <-- at this point you can check the err object and do something else if you'd like
          if (err.status == 401) {
            console.log(`Ohoh, unauthorized`, err);
          } else {
            console.log(`Ohoh, http error`, err);
          }
        }
        return throwError(err);
      }),
    );
  }
}
