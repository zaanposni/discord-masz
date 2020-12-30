import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class ApiCacheService {

  constructor(private api: ApiService) { }

  private data: { [path: string] : Promise<any> } = { };

  getSimpleData(apiPath: string): Promise<any> {
    if ( !( apiPath in this.data) ) {
      this.data[apiPath] = this.api.getSimpleData(apiPath).toPromise();
    }
    return this.data[apiPath];
  }
}
