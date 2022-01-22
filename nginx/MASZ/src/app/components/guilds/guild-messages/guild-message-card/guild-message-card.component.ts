import { Component, Input, OnInit } from '@angular/core';
import { IScheduledMessage } from 'src/app/models/IScheduledMessage';

@Component({
  selector: 'app-guild-message-card',
  templateUrl: './guild-message-card.component.html',
  styleUrls: ['./guild-message-card.component.css']
})
export class GuildMessageCardComponent implements OnInit {

  @Input() message!: IScheduledMessage;

  constructor() { }

  ngOnInit(): void {
  }

}
