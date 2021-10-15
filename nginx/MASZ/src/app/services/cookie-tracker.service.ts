import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie';
import { ReplaySubject } from 'rxjs';
import { DEFAULT_LANGUAGE, DEFAULT_TIMEZONE } from '../config/config';
import { AppSettings } from '../models/AppSettings';

@Injectable({
  providedIn: 'root'
})
export class CookieTrackerService {

  settings: ReplaySubject<AppSettings> = new ReplaySubject<AppSettings>(1);

  constructor(private cookieService: CookieService) {
    this.updateObservable();
  }

  private updateObservable() {
    let settings: AppSettings = {
      language: this.cookieService.hasKey('language') ? this.cookieService.get('language') : DEFAULT_LANGUAGE,
      timezone: this.cookieService.hasKey('timezone') ? this.cookieService.get('timezone') : DEFAULT_TIMEZONE,
    }
    this.settings.next(settings);
  }

  updateCookie(key: string, newValue: string) {
    let currentDate = new Date();
    let futureDate  = new Date(currentDate.getFullYear() + 10, currentDate.getMonth(), currentDate.getDay());
    this.cookieService.put(key, newValue, { httpOnly: false, expires: futureDate, secure: false, sameSite: 'lax' });
    this.updateObservable();
  }

  updateSettings(newSettings: AppSettings) {
    let currentDate = new Date();
    let futureDate  = new Date(currentDate.getFullYear() + 10, currentDate.getMonth(), currentDate.getDay());
    this.cookieService.put('language', newSettings.language, { httpOnly: false, expires: futureDate, secure: false, sameSite: 'lax' });
    this.cookieService.put('timezone', newSettings.timezone, { httpOnly: false, expires: futureDate, secure: false, sameSite: 'lax' });
    this.updateObservable();
  }
}
