import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import * as moment from 'moment';
import { TimezoneService } from 'src/app/services/timezone.service';
import { DEFAULT_TIMEZONE } from 'src/app/config/config';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-date-picker',
  templateUrl: './date-picker.component.html',
  styleUrls: ['./date-picker.component.css']
})
export class DatePickerComponent implements OnInit {

  date = moment();
  currentTimezone: string = DEFAULT_TIMEZONE;

  @Input() dateChangedInParent?: Observable<Date|moment.Moment>;

  @Output() dateChanged = new EventEmitter<moment.Moment>();

  constructor(private timezone: TimezoneService) { }

  ngOnInit(): void {
    this.timezone.selectedTimezone.subscribe(timezone => {
      this.currentTimezone = timezone;
    });
    if (this.dateChangedInParent) {
      this.dateChangedInParent.subscribe(date => {
        this.date = moment(date);
        this.dateChanged.emit(this.date);
      });
    }
  }

  // use date.toISOString() to access date in api format
  public dateChange(event: any) {
    this.date = moment(event.value);
    this.date = this.date.utc(true);
    if (this.currentTimezone !== 'UTC') {
      this.date = this.date.utcOffset(this.currentTimezone, true);
    }
    this.dateChanged.emit(this.date);
  }
}
