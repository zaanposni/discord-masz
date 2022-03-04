import { Component, Input, OnInit } from '@angular/core';
import { IAppealAnswer } from 'src/app/models/IAppealAnswer';

@Component({
  selector: 'app-guild-appeal-question',
  templateUrl: './guild-appeal-question.component.html',
  styleUrls: ['./guild-appeal-question.component.css']
})
export class GuildAppealQuestionComponent implements OnInit {

  @Input() showActionButtons: boolean = false;
  @Input() question?: string;
  @Input() answer?: string;

  constructor() { }

  ngOnInit(): void {
  }

}
