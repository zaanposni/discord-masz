import { Component, OnInit } from '@angular/core';
import { CookieService } from 'ngx-cookie';
import { CookieTrackerService } from './services/cookie-tracker.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

  constructor(private cookieTracker: CookieTrackerService) { }

  ngOnInit(): void {
    this.cookieTracker.currentSettings.subscribe(data => this.darkMode = data.darkMode);
  }

  title = 'masz';
  darkMode: boolean = true;
}
