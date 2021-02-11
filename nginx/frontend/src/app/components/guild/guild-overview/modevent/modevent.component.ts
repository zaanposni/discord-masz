import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AutoModerationEvent } from 'src/app/models/AutoModerationEvent';

declare function collapseTarget(target: any): any;

@Component({
  selector: 'app-modevent',
  templateUrl: './modevent.component.html',
  styleUrls: ['./modevent.component.scss']
})
export class ModeventComponent implements OnInit {

  @Input() moderation: AutoModerationEvent;
  moderationBig: boolean = false;

  iconsMap: { [key: number]: string} = {
    0: 'far fa-envelope-open',
    1: 'far fa-smile-wink',
    2: 'fas fa-user',
    3: 'far fa-file-alt',
    4: 'fas fa-link'
  };
  eventsMap: { [key: number]: string} = {
    0: 'posted an invite',
    1: 'used too many emotes',
    2: 'mentioned too many users',
    3: 'posted too many attachments',
    4: 'posted too many embeds'
  };
  actionsMap: { [key: number]: string} = {
    0: 'No action was taken',
    1: 'Content deleted',
    2: 'Case created',
    3: 'Content deleted and case created'
  }
  // function collapse(item) {
  //   $(item).toggleClass('moderation-big');
  //   
  // }

  constructor(public router: Router) { }

  ngOnInit(): void {
  }

  collapse() {
    this.moderationBig = !this.moderationBig;
    collapseTarget('#collapse-' + this.moderation.id);
  }
} 
