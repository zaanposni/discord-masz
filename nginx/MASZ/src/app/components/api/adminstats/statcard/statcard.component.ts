import { Component, Input, OnInit } from '@angular/core';
import { IServiceStatus } from 'src/app/models/IServiceStatus';

@Component({
  selector: 'app-statcard',
  templateUrl: './statcard.component.html',
  styleUrls: ['./statcard.component.css']
})
export class StatcardComponent implements OnInit {

  @Input() titleKey: string = "";
  @Input() text?: string | number | undefined;
  @Input() emote?: string;
  @Input() renderPing?: IServiceStatus = undefined;
  @Input() warningPingLimit: number = 200;
  @Input() errorPingLimit: number = 400;

  constructor() { }

  ngOnInit(): void {
  }

}
