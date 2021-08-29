import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { AutomodConfig } from 'src/app/models/AutomodConfig';
import { AutoModRuleDefinition } from 'src/app/models/AutoModRuleDefinitions';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { Guild } from 'src/app/models/Guild';
import { GuildChannel } from 'src/app/models/GuildChannel';
import { GuildMotd } from 'src/app/models/GuildMotd';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-automod-config',
  templateUrl: './automod-config.component.html',
  styleUrls: ['./automod-config.component.css']
})
export class AutomodConfigComponent implements OnInit {
  types: AutoModRuleDefinition[] = [
    {
      type: 0,
      title: 'Invites',
      description: 'A message on your guild matches the invite pattern.',
      showLimitField: false,
      showTimeLimitField: false
    },
    {
      type: 1,
      title: 'Emotes',
      description: 'A message on your guild contains too many emotes.',
      showLimitField: true,
      showTimeLimitField: false
    },
    {
      type: 2,
      title: 'Mentions',
      description: 'A message on your guild mentions too many users. Does not include role mentions.',
      showLimitField: true,
      showTimeLimitField: false
    },
    {
      type: 3,
      title: 'Attachments',
      description: 'A message on your guild contains too many attachments.',
      showLimitField: true,
      showTimeLimitField: false
    },
    {
      type: 4,
      title: 'Embeds',
      description: 'A message on your guild contains too many embeds. This includes preview of links.',
      showLimitField: true,
      showTimeLimitField: false
    },
    {
      type: 5,
      title: 'Automoderations',
      description: 'A user triggers too many automoderations in a defined timespan.',
      showLimitField: true,
      showTimeLimitField: true
    },
    {
      type: 6,
      title: 'Custom filter',
      description: 'A user uses too many words defined in your list.',
      showLimitField: true,
      showTimeLimitField: false,
      showCustomField: true,
      tooltip: 'Words are checked line by line case insensitive.',
      link: 'https://gist.github.com/zaanposni/4f3aa7b29d54005d34eb78f6acfe93eb',
      linkText: 'Example'
    },
    {
      type: 7,
      title: 'Spam',
      description: 'A user on your guild sends too many messages in a timespan.',
      showLimitField: true,
      showTimeLimitField: true,
      timeLimitFieldMessage: 'Time limit (seconds)'
    },

  ];

  public guildId!: string;
  public guildInfo!: Guild;
  public guildChannels!: GuildChannel[];
  public initialConfigs!: Promise<AutomodConfig[]>;

  constructor(private api: ApiService, private toastr: ToastrService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid') as string;
    this.reload();
  }

  reload() {
    this.api.getSimpleData(`/discord/guilds/${this.guildId}`).subscribe((data) => {
      this.guildInfo = data;
    }, () => {
      this.toastr.error("Failed to load guild info.");
    });
    this.api.getSimpleData(`/discord/guilds/${this.guildId}/channels`).subscribe((data: GuildChannel[]) => {
      this.guildChannels = data.filter(x => x.type === "0");
    }, () => {
      this.toastr.error("Failed to load guildchannel info.");
    });
    this.initialConfigs = this.api.getSimpleData(`/guilds/${this.guildId}/automoderationconfig`).toPromise();
  }
}
