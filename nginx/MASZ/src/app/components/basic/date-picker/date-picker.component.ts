import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import * as moment from 'moment';
import { TimezoneService } from 'src/app/services/timezone.service';
import { DEFAULT_TIMEZONE } from 'src/app/config/config';
import { Observable } from 'rxjs';
import { NgxMatDateAdapter } from '@angular-material-components/datetime-picker';

@Component({
  selector: 'app-date-picker',
  templateUrl: './date-picker.component.html',
  styleUrls: ['./date-picker.component.css']
})
export class DatePickerComponent implements OnInit {

  date?: Date = undefined;
  currentTimezone: string = DEFAULT_TIMEZONE;
  invertedCurrentTimezone: string = DEFAULT_TIMEZONE;

  @Input() dateChangedInParent?: Observable<Date|moment.Moment>;
  @Input() placeholder: string = 'DTPicker.ChooseADate';

  @Output() dateChanged = new EventEmitter<moment.Moment>();

  constructor(private timezone: TimezoneService) { }

  ngOnInit(): void {
    this.timezone.selectedTimezone.subscribe(timezone => {
      this.currentTimezone = timezone;
      // special magic (∩｀-´)⊃━☆ﾟ.*･｡ﾟ
      this.invertedCurrentTimezone = timezone.replace('+', '#').replace('-', '+').replace('#', '-');
    });
    if (this.dateChangedInParent) {
      this.dateChangedInParent.subscribe(date => {
        if (date == undefined) {
          this.date = undefined;
          return;
        }
        let momentDate = moment.utc(moment(date));
        if (this.invertedCurrentTimezone !== 'UTC') {
          momentDate = momentDate.utcOffset(this.invertedCurrentTimezone, true);
        }
        this.date = momentDate.toDate();
        this.dateChange({value: this.date});
      });
    }
  }

  // use date.toISOString() to access date in api format
  public dateChange(event: any) {
    let momentDate = moment(event.value).utc(true);
    if (this.currentTimezone !== 'UTC') {
      momentDate = momentDate.utcOffset(this.currentTimezone, true);
    }
    this.dateChanged.emit(momentDate);
  }
}
