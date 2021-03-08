import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie';
import { BehaviorSubject } from 'rxjs';
import { AppSettings } from '../models/AppSettings';

@Injectable({
  providedIn: 'root'
})
export class CookieTrackerService {

  private settingsSource = new BehaviorSubject<AppSettings>({ darkMode: true, showSuggestions: true });
  currentSettings = this.settingsSource.asObservable();

  constructor(private cookieService: CookieService) {
    this.updateObservable();
  }

  private updateObservable() {
    let settings: AppSettings = {
      darkMode: (!this.cookieService.hasKey('darkmode')) || this.cookieService.get('darkmode') === 'true',
      showSuggestions: (!this.cookieService.hasKey('suggestions')) || this.cookieService.get('suggestions') === 'true',
    }
    this.settingsSource.next(settings);
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
    this.cookieService.put('darkmode', newSettings.darkMode ? 'true' : 'false', { httpOnly: false, expires: futureDate, secure: false, sameSite: 'lax' });
    this.cookieService.put('suggestions', newSettings.showSuggestions ? 'true' : 'false', { httpOnly: false, expires: futureDate, secure: false, sameSite: 'lax' });
    this.updateObservable();
  }
}
