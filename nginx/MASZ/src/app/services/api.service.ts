import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { API_URL, APP_BASE_URL } from '../config/config';
import { APIEnumTypes } from '../models/APIEmumTypes';
import { APIEnum } from '../models/APIEnum';
import { EnumManagerService } from './enum-manager.service';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  private enumManager?: EnumManagerService = undefined;
  private apiErrors: APIEnum[] = [];

  constructor(private http: HttpClient, private toastr: ToastrService, private injector: Injector) {
    setTimeout(() => {
      this.enumManager = this.injector.get(EnumManagerService);
      this.enumManager.getEnum(APIEnumTypes.APIERRORS).subscribe((apiErrors: APIEnum[]) => {
        this.apiErrors = apiErrors;
      });
    });
  }

  public getSimpleData(path: string, includeBasePath: boolean = true, httpParams: HttpParams = new HttpParams(), handleApiError: boolean = false): Observable<any> {
    if (includeBasePath) {
      path = API_URL + path;
    } else {
      path = APP_BASE_URL + path;
    }

    return this.http.get(path, { params: httpParams }).pipe(
      map(res => {
        return res;
      }),
      catchError((error) => { if (handleApiError) { return this.handleError(error, this.toastr) } return throwError(error) })
    );
  }

  public deleteData(path: string, httpParams: HttpParams = new HttpParams(), includeBasePath: boolean = true, handleApiError: boolean = false): Observable<any> {
    if (includeBasePath) {
      path = API_URL + path;
    } else {
      path = APP_BASE_URL + path;
    }

    return this.http.delete(path, { params: httpParams }).pipe(
      map(res => {
        return res;
      }),
      catchError((error) => { if (handleApiError) { return this.handleError(error, this.toastr) } return throwError(error) })
    );
  }

  public postSimpleData(path: string, body: any, httpParams: HttpParams = new HttpParams(), includeBasePath: boolean = true, handleApiError: boolean = false): Observable<any> {
    if (includeBasePath) {
      path = API_URL + path;
    } else {
      path = APP_BASE_URL + path;
    }

    return this.http.post(path, body, { params: httpParams }).pipe(
      map(res => {
        return res;
      }),
      catchError((error) => {if (handleApiError) { return this.handleError(error, this.toastr) } return throwError(error) })
    );
  }

  public putSimpleData(path: string, body: any, httpParams: HttpParams = new HttpParams(), includeBasePath: boolean = true, handleApiError: boolean = false): Observable<any> {
    if (includeBasePath) {
      path = API_URL + path;
    } else {
      path = APP_BASE_URL + path;
    }

    return this.http.put(path, body, { params: httpParams }).pipe(
      map(res => {
        return res;
      }),
      catchError((error) => {if (handleApiError) { return this.handleError(error, this.toastr) } return throwError(error) })
    );
  }

  public postFile(path: string, fileToUpload: File, includeBasePath: boolean = true, handleApiError: boolean = false): Observable<any> {
    if (includeBasePath) {
      path = API_URL + path;
    } else {
      path = APP_BASE_URL + path;
    }

    const formData: FormData = new FormData();
    formData.append('File', fileToUpload, fileToUpload.name);

    return this.http.post(path, formData,).pipe(
      map(res => {
        return res;
      }),
      catchError((error) => {if (handleApiError) { return this.handleError(error, this.toastr) } return throwError(error) })
    );
  }

  private handleError(error: HttpErrorResponse, toastr: ToastrService): Observable<never> {
    let msg = '';
    if (error.error instanceof ErrorEvent) {
      // client-side error
      msg = error.error.message;
    } else {
      // server-side error
      if (typeof error.error === 'object' && error.error !== null) {
        if (('errors') in error.error) {
          for (let key in error.error['errors']) {
            toastr.error(error.error['errors'][key].join(' '), key);
          }
        } else
        if ('title' in error.error) {
          toastr.error(error.error.title, `${error.status}: ${error.statusText}`);  // if title field from asp net standard responses is included
        } else if ('customMASZError' in error.error) {
          if (this.apiErrors.find(x => x.key === error.error.customMASZError)) {
            toastr.error(this.apiErrors?.find(x => x.key === error.error.customMASZError)?.value ?? error.error?.message ?? 'Unknown');
          } else {
            toastr.error(error.message, `${error.status}: ${error.statusText}`);
          }
        }
        else {
          toastr.error(error.message, `${error.status}: ${error.statusText}`);
        }
      } else if (error.error !== null) {
        toastr.error(error.error, `${error.status}: ${error.statusText}`);
      } else {
        toastr.error(error.message, `${error.status}: ${error.statusText}`);
      }
      msg = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    return throwError(msg);
  }
}
