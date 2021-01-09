import { CommonModule, DOCUMENT } from '@angular/common';
import { HttpParams } from '@angular/common/http';
import { Component, Inject, Input, OnInit, Renderer2 } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Toast, ToastrService } from 'ngx-toastr';
import { Observable, Subject } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { AppUser } from 'src/app/models/AppUser';
import { AutoModerationEvent } from 'src/app/models/AutoModerationEvent';
import { AutoModerationEventInfo } from 'src/app/models/AutoModerationEventInfo';
import { DiscordUser } from 'src/app/models/DiscordUser';
import { FileInfo } from 'src/app/models/FileInfo';
import { Guild } from 'src/app/models/Guild';
import { ModCase } from 'src/app/models/ModCase';
import { ApiCacheService } from 'src/app/services/api-cache.service';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';
import Swal from 'sweetalert2'

declare function initDatatables(): any;

@Component({
  selector: 'app-guild-overview',
  templateUrl: './guild-overview.component.html',
  styleUrls: ['./guild-overview.component.scss']
})
export class GuildOverviewComponent implements OnInit {

  guildId!: string | null;
  guild!: Promise<Guild>;
  modCases!: Promise<ModCase[]>;
  activePunishments!: Promise<ModCase[]>;
  moderationEventsInfo!: Promise<AutoModerationEventInfo>;
  moderationEvents: AutoModerationEvent[] = new Array<AutoModerationEvent>();
  isModOrHigher: boolean = false;
  isAdminOrHigher: boolean = false;
  users: { [key: string]: Promise<DiscordUser> } = {};

  iconsMap: { [key: number]: string} = {
    0: 'fas fa-link'
  };

  eventsMap: { [key: number]: string} = {
    0: 'posted an invite'
  };

  actionsMap: { [key: number]: string} = {
    0: 'No action was taken',
    1: 'Content deleted',
    2: 'Case created',
    3: 'Content deleted and case created'
  }

  private lastDate: Date = undefined;
  isEqualToLastDate(date: Date): boolean {
    let b;
    if (this.lastDate) {
      b = new Date(date).getDate() === this.lastDate.getDate();
    } else {
      b = false;
    }
    this.lastDate = new Date(date);
    return b;
  }
  startPage = 1;

  constructor(private cache: ApiCacheService, private route: ActivatedRoute, private auth: AuthService, private toastr: ToastrService,
     private api: ApiService, public router: Router) { }

  ngOnInit(): void {

    this.guildId = this.route.snapshot.paramMap.get('guildid');
    this.auth.getUserProfile().subscribe((data) => {
      this.isModOrHigher = data.modGuilds.find(x => x.id === this.guildId) !== undefined || data.adminGuilds.find(x => x.id === this.guildId) !== undefined || data.isAdmin;
      this.isAdminOrHigher = data.adminGuilds.find(x => x.id === this.guildId) !== undefined || data.isAdmin;
    });
    this.guild = this.cache.getSimpleData(`/discord/guilds/${this.guildId}`);
    this.modCases = this.api.getSimpleData(`/modcases/${this.guildId}`).toPromise();
    this.modCases.then(
      (data) => {
        initDatatables();
        data.forEach(element => {
          this.users[element.userId] = this.cache.getSimpleData(`/discord/users/${element.userId}`, true, null, false);
        });
      });
    this.activePunishments = this.api.getSimpleData(`/modcases/${this.guildId}`).pipe(
      map(items => items.filter((item: { punishmentActive: boolean; }) => item.punishmentActive === true))
    ).toPromise();
    
    this.moderationEventsInfo = this.api.getSimpleData(`/guilds/${this.guildId}/automoderations`).toPromise().then((data) => {
      data.events.forEach((element: AutoModerationEvent) => {
        this.moderationEvents.push(element);        
      });
      return data;
    });
  }

  loadMoreData(): void {
    let params = new HttpParams()
          .set('startPage', this.startPage.toString());

    this.startPage++;
    this.api.getSimpleData(`/guilds/${this.guildId}/automoderations`, true, params).subscribe((data) => {
      data.events.forEach((element: AutoModerationEvent) => {
        this.moderationEvents.push(element);        
      });
    })
  }
}
