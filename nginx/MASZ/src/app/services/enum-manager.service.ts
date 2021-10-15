import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ReplaySubject } from 'rxjs';
import { LANGUAGES } from '../config/config';
import { APIEnumTypes } from '../models/APIEmumTypes';
import { APIEnum } from '../models/APIEnum';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class EnumManagerService {

  private cachedEnums: { [key: string]: ReplaySubject<APIEnum[]> } = {};

  constructor(private api: ApiService, private translator: TranslateService) {
    this.translator.onLangChange.subscribe(() => {
      this.renewAllEnums();
    });
  }

  private getCurrentLanguage(): number {
    return LANGUAGES.find(x => x.language === this.translator.currentLang)?.apiValue ?? 0;
  }

  public getEnum(enumName: APIEnumTypes|string, renew: boolean = false) {
    let reload = renew;
    if (! (enumName in this.cachedEnums)) {
      reload = true;
      this.cachedEnums[enumName] = new ReplaySubject<APIEnum[]>(1);
    }
    if (reload) {
      const params = new HttpParams().set('language', this.getCurrentLanguage());
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
