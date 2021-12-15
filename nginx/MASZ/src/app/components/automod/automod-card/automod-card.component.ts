import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { APIEnumTypes } from 'src/app/models/APIEmumTypes';
import { AutoModerationEvent } from 'src/app/models/AutoModerationEvent';
import { EnumManagerService } from 'src/app/services/enum-manager.service';

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
    3: 'attach_file',
    4: 'description',
    5: 'history',
    6: 'do_not_disturb',
    7: 'manage_search',
    8: 'text_fields',
    9: 'link'
  };
  action: string = "Unknown";
  event: string = "Unknown";

  constructor(public router: Router, private enumManager: EnumManagerService) { }

  ngOnInit(): void {
    this.moderation.messageContent = this.moderation.messageContent.replace(/\n/g, "\\n");
    this.enumManager.getEnum(APIEnumTypes.AUTOMODACTION).subscribe(data => {
      this.action = data?.find(x => x.key == this.moderation.autoModerationAction)?.value ?? "Unknown";
    });
    this.enumManager.getEnum(APIEnumTypes.AUTOMODTYPE).subscribe(data => {
      this.event = data?.find(x => x.key == this.moderation.autoModerationType)?.value ?? "Unknown";
    });
  }

}
