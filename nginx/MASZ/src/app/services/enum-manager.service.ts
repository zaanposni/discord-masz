import { Injectable } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { APIEnumTypes } from '../models/APIEmumTypes';
import { APIEnum } from '../models/APIEnum';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class EnumManagerService {

  private cachedEnums: { [key: string]: ReplaySubject<APIEnum[]> } = {};

  constructor(private api: ApiService) { }

  public getEnum(enumName: APIEnumTypes, renew: boolean = false) {
    if ((! (enumName in this.cachedEnums)) || renew) {
      this.cachedEnums[enumName] = new ReplaySubject<APIEnum[]>(1);
      this.api.getSimpleData(`/enums/${enumName}`).subscribe(data => {
        this.cachedEnums[enumName].next(data);
      });
    }
    return this.cachedEnums[enumName].asObservable();
  }

  renewAllEnums() {
    for (let key in this.cachedEnums) {
      this.cachedEnums[key].complete();
      delete this.cachedEnums[key];
    }
  }
}
