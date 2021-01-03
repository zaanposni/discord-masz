import { Component, OnInit } from '@angular/core';
import { CookieService } from 'ngx-cookie';
import { ToastrService } from 'ngx-toastr';
import { AppComponent } from 'src/app/app.component';
import { AppSettings } from 'src/app/models/AppSettings';
import { CookieTrackerService } from 'src/app/services/cookie-tracker.service';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {

  currentSettings: AppSettings = { darkMode: true, showSuggestions: true };

  constructor(private cookieTracker: CookieTrackerService, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.cookieTracker.currentSettings.subscribe(data => this.currentSettings = data);
  }

  onChanges() {
    this.cookieTracker.updateSettings(this.currentSettings);
    this.toastr.success('Changes saved');
  }
}
