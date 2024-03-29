import { Injectable } from "@angular/core";
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpResponse
} from "@angular/common/http";
import { Observable, throwError } from "rxjs";
import { catchError } from "rxjs/operators";

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError(error => {
        if (error.status == 401) {
          return throwError(error.statusText);
        }
        if (error instanceof HttpResponse) {
          const applicationError = error.headers.get("Application-Error");
          if (applicationError) return throwError(applicationError);
        }
        const serverError = error.Error;
        let modalStateErrors = "";
        if (serverError && typeof error === "object") {
          for (const key in serverError) {
            if (serverError[key]) {
              modalStateErrors += serverError[key] + "\n";
            }
          }
          return throwError(modalStateErrors || serverError || "Server Error");
        }
      })
    );
  }
}