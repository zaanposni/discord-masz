import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Moment } from 'moment';
import { ReplaySubject } from 'rxjs';
import { IScheduledMessage } from 'src/app/models/IScheduledMessage';

@Component({
  selector: 'app-scheduled-message-edit-dialog',
  templateUrl: './scheduled-message-edit-dialog.component.html',
  styleUrls: ['./scheduled-message-edit-dialog.component.css']
})
export class ScheduledMessageEditDialogComponent implements OnInit {

  public initRows = 1;
  public scheduledForChangedForPicker: ReplaySubject<Date> = new ReplaySubject<Date>(1);
  constructor(@Inject(MAT_DIALOG_DATA) public message: IScheduledMessage) { }

  ngOnInit(): void {
    this.initRows = Math.min(this.message.content.split(/\r\n|\r|\n/).length, 15);
    this.scheduledForChangedForPicker.next(this.message.scheduledFor);
  }

  dateChanged(date: Moment) {
    this.message.scheduledFor = date as any;
  }
}
