import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AutoModerationEvent } from 'src/app/models/AutoModerationEvent';

@Component({
  selector: 'app-automod-card',
  templateUrl: './automod-card.component.html',
  styleUrls: ['./automod-card.component.css']
})
export class AutomodCardComponent implements OnInit {

  @Input() moderation!: AutoModerationEvent;

  iconsMap: { [key: number]: string} = {
    0: 'forward_to_inbox',
    1: 'sentiment_satisfied_alt',
    2: 'person',
    3: 'description',
    4: 'attach_file',
    5: 'history',
    6: 'do_not_disturb'
  };
  eventsMap: { [key: number]: string} = {
    0: 'posted an invite',
    1: 'used too many emotes',
    2: 'mentioned too many users',
    3: 'posted too many attachments',
    4: 'posted too many embeds',
    5: 'triggered too many automoderations',
    6: 'used too many unallowed words'
  };
  actionsMap: { [key: number]: string} = {
    0: 'No action was taken',
    1: 'Content deleted',
    2: 'Case created',
    3: 'Content deleted and case created'
  }

  constructor(public router: Router) { }

  ngOnInit(): void {
  }

}
