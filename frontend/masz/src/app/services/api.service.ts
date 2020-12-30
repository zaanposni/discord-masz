import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { API_URL, APP_BASE_URL } from '../config/config';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  constructor(private http: HttpClient, private toastr: ToastrService) { }

  public getSimpleData(path: string, includeBasePath: boolean = true): Observable<any> {
    if (includeBasePath) {
      path = API_URL + path;
    } else {
      path = APP_BASE_URL + path;
    }

    return this.http.get(path).pipe(
      map(res => {
        return res;
      }),
      catchError((error) => { return this.handleError(error, this.toastr) })
    );
  }

  public deleteData(path: string, httpParams: HttpParams = new HttpParams(), includeBasePath: boolean = true): Observable<any> {
    if (includeBasePath) {
      path = API_URL + path;
    } else {
      path = APP_BASE_URL + path;
    }

    return this.http.delete(path, { params: httpParams }).pipe(
      map(res => {
        return res;
      }),
      catchError((error) => { return this.handleError(error, this.toastr) })
    );
  }

  public postSimpleData(path: string, body: any, httpParams: HttpParams = new HttpParams(), includeBasePath: boolean = true): Observable<any> {
    if (includeBasePath) {
      path = API_URL + path;
    } else {
      path = APP_BASE_URL + path;
    }

    return this.http.post(path, body, { params: httpParams }).pipe(
      map(res => {
        return res;
      }),
      catchError((error) => { return this.handleError(error, this.toastr) })
    );
  }

  public postFile(path: string, fileToUpload: File, includeBasePath: boolean = true): Observable<any> {
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
      catchError((error) => { return this.handleError(error, this.toastr) })
    );
  }

  private handleError(error: HttpErrorResponse, toastr: ToastrService): Observable<never> {
    let msg = '';
    if (error.error instanceof ErrorEvent) {
      // client-side error
      msg = error.error.message;
    } else {
      // server-side error
      toastr.error(error.error, `${error.status}: ${error.statusText}`)
      msg = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    return throwError(msg);
  }
}
