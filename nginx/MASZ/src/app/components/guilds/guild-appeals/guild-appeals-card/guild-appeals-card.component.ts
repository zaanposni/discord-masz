import { Component, Input, OnInit } from '@angular/core';
import { IAppealView } from 'src/app/models/IAppealView';

@Component({
  selector: 'app-guild-appeals-card',
  templateUrl: './guild-appeals-card.component.html',
  styleUrls: ['./guild-appeals-card.component.css']
})
export class GuildAppealsCardComponent implements OnInit {

  @Input() entry!: IAppealView;
  constructor() { }

  ngOnInit(): void {
  }

}
