import { HttpParams } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AutomodConfig } from 'src/app/models/AutomodConfig';
import { AutoModRuleDefinition } from 'src/app/models/AutoModRuleDefinitions';
import { Guild } from 'src/app/models/Guild';
import { GuildChannel } from 'src/app/models/GuildChannel';
import { GuildRole } from 'src/app/models/GuildRole';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-automod-rule',
  templateUrl: './automod-rule.component.html',
  styleUrls: ['./automod-rule.component.scss']
})
export class AutomodRuleComponent implements OnInit {

  types: { [key: string]: AutoModRuleDefinition } = {
    '0': {
      uniqueLabel: 'invite',
      title: 'Invites',
      description: 'A message on your guild matches the invite pattern.',
      showLimitField: false
    },
    '1': {
      uniqueLabel: 'emote',
      title: 'Emotes',
      description: 'A message on your guild contains too many emotes.',
      showLimitField: true
    },
    '2': {
      uniqueLabel: 'mention',
      title: 'Mentions',
      description: 'A message on your guild mentions too many users. Does not include role mentions.',
      showLimitField: true
    },
    '3': {
      uniqueLabel: 'attachment',
      title: 'Attachments',
      description: 'A message on your guild contains too many attachments.',
      showLimitField: true
    },
    '4': {
      uniqueLabel: 'embed',
      title: 'Embeds',
      description: 'A message on your guild contains too many embeds. This includes preview of links.',
      showLimitField: true
    }
  };

  punishmentMap: { [key: string]: { [k: string]: any } } = {
    '0' : {
      'punishment': 'Warn',
      'punishmentType': 0
    },
    '1' : {
      'punishment': 'Mute',
      'punishmentType': 1
    },
    '2' : {
      'punishment': 'TempMute',
      'punishmentType': 1
    },
    '3' : {
      'punishment': 'Kick',
      'punishmentType': 2
    },
    '4' : {
      'punishment': 'Ban',
      'punishmentType': 3
    },
    '5' : {
      'punishment': 'TempBan',
      'punishmentType': 3
    }
  };

  title: string;
  uniqueLabel: string;
  description: string;
  showLimitField: boolean;

  @Input() guildId: string;
  @Input() type: string;

  @Input() limit: string;

  @Input() excludeRoles: boolean = false;
  @Input() roleToExclude!: any;
  excludedRoles: GuildRole[] = [];

  @Input() excludeChannels: boolean = false;
  @Input() channelToExclude!: any;
  excludedChannels: GuildChannel[] = [];

  currentGuild: Guild;
  currentGuildChannels: GuildChannel[];

  automodEnabled: boolean = false;
  @Input() config: AutomodConfig = undefined;
  @Input() sendDmNotification: boolean = false;
  @Input() sendPublicNotification: boolean = false;
  @Input() deleteMessage: boolean = false;
  @Input() createCase: boolean = false;
  @Input() punishment: string = '0';
  @Input() punishmentDuration: string = '0';

  constructor(private api: ApiService, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.title = this.types[this.type].title;
    this.uniqueLabel = this.types[this.type].uniqueLabel;
    this.description = this.types[this.type].description;
    this.showLimitField = this.types[this.type].showLimitField;

    this.api.getSimpleData(`/discord/guilds/${this.guildId}`).subscribe((data) => {
      this.currentGuild = data;
      this.reload(true);
    });
    this.api.getSimpleData(`/discord/guilds/${this.guildId}/channels`).subscribe((data: GuildChannel[]) => {
      this.currentGuildChannels = data.filter((item: GuildChannel) => item.type === '0');
      this.reload(true);
    });
  }

  generateRoleColor(role: GuildRole): string {
    return '#' + role.color.toString(16) + ' !important';
  }

  reload(cache: boolean = false) {    
    this.api.getSimpleData(`/guilds/${this.guildId}/automoderationconfig/${this.type}`, true, new HttpParams(), false).toPromise().then((data) => {
      this.config = data;
      this.automodEnabled = true;

      this.limit = this.config.limit?.toString();

      this.config.ignoreRoles.forEach(element => {
        this.roleToExclude = this.currentGuild.roles.find(r => { return r.id === element });
        this.excludeRole();
      });
      this.excludeRoles = this.excludedRoles.length !== 0;
      this.roleToExclude = null;

      this.config.ignoreChannels.forEach(element => {
        this.channelToExclude = this.currentGuildChannels.find(r => { return r.id === element });
        this.excludeChannel();
      });
      this.excludeChannels = this.excludedChannels.length !== 0;
      this.channelToExclude = null;

      this.sendDmNotification = this.config.sendDmNotification;
      this.sendPublicNotification = this.config.sendPublicNotification;
      this.deleteMessage = this.config.autoModerationAction === 1 || this.config.autoModerationAction === 3;
      this.createCase = this.config.autoModerationAction === 2 || this.config.autoModerationAction === 3;
      this.punishmentDuration = this.config.punishmentDurationMinutes.toString();

      this.punishmentDuration = '';
      switch(this.config.punishmentType) {
        case 1:
          if (this.config.punishmentDurationMinutes) {
            this.punishment = '2';
            this.punishmentDuration = this.config.punishmentDurationMinutes.toString();
          } else {
            this.punishment = '1';
          }
          break;
        case 2:
          this.punishment = '3';
          break;
        case 3:
          if (this.config.punishmentDurationMinutes) {
            this.punishment = '5';
            this.punishmentDuration = this.config.punishmentDurationMinutes.toString();
          } else {
            this.punishment = '4';
          }
          break;
        default:
          this.punishment = '0';
      }
    }, (error) => {  // assumes 404 for no config set yet
      this.automodEnabled = false;
    });
  }

  isNumber(n: any): boolean { return !isNaN(parseFloat(n)) && !isNaN(n - 0) }

  saveChanges() {
    if (!this.automodEnabled) {
      this.api.deleteData(`/guilds/${this.guildId}/automoderationconfig/${this.type}`, new HttpParams(), true, false).subscribe((data) => {
        this.toastr.success("Changes saved.");
        this.reload();
      }, (error) => {
        console.log('Failed to update config.');
      });
      return;
    }

    if(!this.isNumber(this.punishmentDuration) && this.punishmentDuration) {
      this.toastr.error('Please enter a valid number as duration.');
      return;
    } else {
      if (+this.punishmentDuration < 0) {
        this.toastr.error('Please enter a valid duration greater than or equal zero.');
        return;
      }
    }

    if (this.showLimitField) {
      if(!this.isNumber(this.limit)) {
        this.toastr.error('Please enter a valid number as limit.');
        return;
      } else {
        if (+this.limit < 0) {
          this.toastr.error('Please enter a valid limit greater than or equal zero.');
          return;
        }
      }
    }

    let data: any = {
      'AutoModerationType': +this.type,
      'PunishmentType': this.punishmentMap[this.punishment]['punishmentType'],
      'PunishmentDurationMinutes': +this.punishmentDuration,
      'IgnoreChannels': this.excludedChannels.map((data) => data.id),
      'IgnoreRoles': this.excludedRoles.map((data) => data.id),
      'SendDmNotification': this.sendDmNotification,
      'SendPublicNotification': this.sendPublicNotification
    }

    let action = 0;
    if (this.deleteMessage) {
      action++;
    }
    if (this.createCase) {
      action += 2;
    }
    data['AutoModerationAction'] = action;

    if(this.showLimitField) {
      data['Limit'] = +this.limit;
    }

    this.api.putSimpleData(`/guilds/${this.guildId}/automoderationconfig`, data).subscribe((data) => {
      this.toastr.success("Changes saved.");
      this.reload();
    }, (error) => {
      this.toastr.error('Failed to update config.', 'Something went wrong')
    });
  }

  onExcludeRolesChange() {
    if (!this.excludeRoles) {
      this.excludedRoles = [];
    }
  }

  excludeRole() {
    if (this.roleToExclude && this.excludedRoles.indexOf(this.roleToExclude) === -1) {
      this.excludedRoles.push(this.roleToExclude);
    }
  }

  undoExcludeRole(role: GuildRole) {
    let index = this.excludedRoles.indexOf(role, 0);
    if (index > -1) {
      this.excludedRoles.splice(index, 1);
    }
  }

  onExcludeChannelsChange() {
    if (!this.excludeChannels) {
      this.excludedChannels = [];
    }
  }

  excludeChannel() {
    if (this.channelToExclude && this.excludedChannels.indexOf(this.channelToExclude) === -1) {
      this.excludedChannels.push(this.channelToExclude);
    }
  }

  undoExcludeChannel(channel: GuildChannel) {
    let index = this.excludedChannels.indexOf(channel, 0);
    if (index > -1) {
      this.excludedChannels.splice(index, 1);
    }
  }
}
