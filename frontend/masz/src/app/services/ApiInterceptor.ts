import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from "@angular/common/http";
import { AuthService } from "./auth.service";
import { ENABLE_CORS } from "../config/config";
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

@Injectable()

export class ApiInterceptor implements HttpInterceptor {
    constructor(private authService: AuthService, private apiService: ApiService) { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
      if (ENABLE_CORS) {
        req = req.clone({
          withCredentials: true
        });
      }
      return next.handle(req);
    }
}
