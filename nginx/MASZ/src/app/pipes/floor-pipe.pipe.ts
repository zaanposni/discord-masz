import { Pipe, PipeTransform } from '@angular/core';

@Pipe({name: 'floor'})
export class FloorPipePipe implements PipeTransform {

  /**
     *
     * @param value
     * @returns {number}
     */
  transform(value?: number): number {
    if (value != null && value != undefined) {
      return Math.floor(value);
    } else {
      return 0;
    }
  }
}
