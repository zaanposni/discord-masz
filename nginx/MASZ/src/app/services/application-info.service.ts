import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { IApplicationInfo } from '../models/IApplicationInfo';

@Injectable({
  providedIn: 'root'
})
export class ApplicationInfoService {

  currentApplicationInfo: ReplaySubject<IApplicationInfo> = new ReplaySubject<IApplicationInfo>(1);

  constructor() { }

  infoChanged(data: IApplicationInfo) {
    this.currentApplicationInfo.next(data);
    if (data?.iconUrl) {
      document.getElementById('favicon')?.setAttribute('href', data.iconUrl);
    }
    if (data?.name) {
      let tabtitle = document.getElementById('tabtitle');
      if (tabtitle!== null) {
        tabtitle.innerText = data.name;
      }
    }
  }
}
