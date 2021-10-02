import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TimezoneService {

  selectedTimezone: ReplaySubject<string> = new ReplaySubject<string>(1);

  constructor() { }

  timezoneChanged(data: string) {
    this.selectedTimezone.next(data);
  }
}
