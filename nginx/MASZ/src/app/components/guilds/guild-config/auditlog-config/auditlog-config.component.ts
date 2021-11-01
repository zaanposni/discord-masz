import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { Guild } from 'src/app/models/Guild';
import { GuildChannel } from 'src/app/models/GuildChannel';
import { IGuildAuditLogConfig } from 'src/app/models/IGuildAuditLogConfig';
import { IGuildAuditLogRuleDefinition } from 'src/app/models/IGuildAuditLogRuleDefinition';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-auditlog-config',
  templateUrl: './auditlog-config.component.html',
  styleUrls: ['./auditlog-config.component.css']
})
export class AuditlogConfigComponent implements OnInit {

  types: IGuildAuditLogRuleDefinition[] = [
    {
      type: 0,
      key: 'MessageSent'
    },
    {
      type: 1,
      key: 'MessageUpdated'
    },
    {
      type: 2,
      key: 'MessageDeleted'
    },
    {
      type: 3,
      key: 'UsernameUpdated'
    },
    {
      type: 4,
      key: 'AvatarUpdated'
    },
    {
      type: 5,
      key: 'NicknameUpdated'
    },
    {
      type: 6,
      key: 'MemberRolesUpdated'
    },
    {
      type: 7,
      key: 'MemberJoined'
    },
    {
      type: 8,
      key: 'MemberRemoved'
    },
    {
      type: 9,
      key: 'BanAdded'
    },
    {
      type: 10,
      key: 'BanRemoved'
    },
    {
      type: 11,
      key: 'InviteCreated'
    },
    {
      type: 12,
      key: 'InviteDeleted'
    },
    {
      type: 13,
      key: 'ThreadCreated'
    }
  ];

  public guildId!: string;
  public guildInfo!: Guild;
  public guildChannels!: GuildChannel[];
  public initialConfigs!: Promise<IGuildAuditLogConfig[]>;

  constructor(private api: ApiService, private toastr: ToastrService, private route: ActivatedRoute, private translator: TranslateService) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.guildId = params.get('guildid') as string;
      this.reload();
    });
  }

  reload() {
    this.api.getSimpleData(`/discord/guilds/${this.guildId}`).subscribe((data: Guild) => {
      data.roles = data.roles.sort((a, b) => (a.position < b.position) ? 1 : -1);
      this.guildInfo = data;
    }, () => {
      this.toastr.error(this.translator.instant('GuildAuditLogConfig.FailedToLoadGuild'));
    });

    this.api.getSimpleData(`/discord/guilds/${this.guildId}/channels`).subscribe((data: GuildChannel[]) => {
      this.guildChannels = data.filter(x => x.type === 0).sort((a, b) => (a.position > b.position) ? 1 : -1);
    }, () => {
      this.toastr.error(this.translator.instant('GuildAuditLogConfig.FailedToLoadChannels'));
    });

    this.initialConfigs = this.api.getSimpleData(`/guilds/${this.guildId}/auditlog`).toPromise();
  }
}
