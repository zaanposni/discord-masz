import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { APIEnumTypes } from '../models/APIEmumTypes';
import { APIEnum } from '../models/APIEnum';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class EnumManagerService {

  private cachedEnums: { [key: string]: ReplaySubject<APIEnum[]> } = {};
  private langauge: number = 0;  // TODO: replace with language manager and enum

  constructor(private api: ApiService) { }

  public getEnum(enumName: APIEnumTypes|string, renew: boolean = false) {
    let reload = renew;
    if (! (enumName in this.cachedEnums)) {
      reload = true;
      this.cachedEnums[enumName] = new ReplaySubject<APIEnum[]>(1);
    }
    if (reload) {
      const params = new HttpParams().set('language', this.langauge.toString());
      this.api.getSimpleData(`/enums/${enumName}`, true, params).subscribe(data => {
        this.cachedEnums[enumName].next(data);
      });
    }
    return this.cachedEnums[enumName].asObservable();
  }

  renewAllEnums() {
    for (let key in this.cachedEnums) {
      this.getEnum(key, true);
    }
  }
}
