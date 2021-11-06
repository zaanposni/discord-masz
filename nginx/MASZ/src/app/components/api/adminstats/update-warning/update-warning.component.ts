import { Component, Input, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { IImageVersion } from 'src/app/models/IImageVersion';

@Component({
  selector: 'app-update-warning',
  templateUrl: './update-warning.component.html',
  styleUrls: ['./update-warning.component.css']
})
export class UpdateWarningComponent implements OnInit {

  @Input() newestVersionObservable?: Observable<IImageVersion> = undefined;
  public newestVersion?: IImageVersion = undefined;

  constructor() { }

  ngOnInit(): void {
    if (this.newestVersionObservable) {
      this.newestVersionObservable.subscribe(version => {
        this.newestVersion = version;
      });
    }
  }
}
