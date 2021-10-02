import { Pipe, PipeTransform } from '@angular/core';
import * as moment from 'moment';

@Pipe({ name: 'dateFormat' })
export class DateFormatPipe implements PipeTransform {
  transform(value: Date | moment.Moment, dateFormat: string, zone: string = "UTC"): any {
    return moment(value).utc(true).utcOffset(zone).format(dateFormat);
  }
}