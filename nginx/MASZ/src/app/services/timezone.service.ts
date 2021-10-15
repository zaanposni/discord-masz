import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import * as moment from 'moment';
import { ReplaySubject } from 'rxjs';
import { DEFAULT_TIMEZONE, LANGUAGES } from '../config/config';

@Injectable({
  providedIn: 'root'
})
export class TimezoneService {

  timezone: string = 'UTC';
  format: string = "DD MMMM Y";
  formatTime: string = "DD MMMM Y HH:mm:ss";

  selectedTimezone: ReplaySubject<string> = new ReplaySubject<string>(1);

  constructor(private translator: TranslateService) {
    this.translator.onLangChange.subscribe(() => {
      this.update();
    });
  }

  timezoneChanged(data: string) {
    this.selectedTimezone.next(data);
    this.timezone = data;
  }

  private update() {
    let currentLang = this.translator.currentLang !== undefined ? this.translator.currentLang : this.translator.defaultLang;
    let currentConfig = LANGUAGES.find(x => x.language === currentLang);
    if (currentConfig !== undefined) {
      this.format = currentConfig.dateFormat;
      this.formatTime = currentConfig.dateTimeFormat;
    } else {
      this.format = "DD MMMM Y";
      this.formatTime = "DD MMMM Y HH:mm:ss";
    }
  }

  convertNearlyAnyDateToLocaleString(date: any, withTime: boolean = true): string {
    if (! (date instanceof Date)) {
      date = new Date(date);
    }
    return this.convertNearlyAnyMomentToLocaleString(date, withTime);
  }

  convertNearlyAnyMomentToLocaleString(date: any, withTime: boolean = true): string {
    return moment(date).utc(true).utcOffset(this.timezone).format(withTime ? this.formatTime : this.format);
  }
}
