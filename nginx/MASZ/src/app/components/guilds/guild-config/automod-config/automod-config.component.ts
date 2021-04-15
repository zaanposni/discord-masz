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
    }
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
