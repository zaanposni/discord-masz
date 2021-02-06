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
import { ModCaseTable } from 'src/app/models/ModCaseTable';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';
import Swal from 'sweetalert2'

declare function initModCaseTable(): any;
declare function initPunishmentTable(): any;

declare function cExcludeAutoModeration(): any;
declare function pExcludeAutoModeration(): any;
declare function cResetAutoModeration(): any;
declare function pResetAutoModeration(): any;

declare function cExcludePermaPunishments(): any;
declare function pExcludePermaPunishments(): any;
declare function cResetPermaPunishments(): any;
declare function pResetPermaPunishments(): any;

@Component({
  selector: 'app-guild-overview',
  templateUrl: './guild-overview.component.html',
  styleUrls: ['./guild-overview.component.scss']
})
export class GuildOverviewComponent implements OnInit {

  guildId!: string | null;
  guild!: Promise<Guild>;
  casesTable!: Promise<ModCaseTable[]>;
  punishmentTable!: Promise<ModCaseTable[]>;
  moderationEventsInfo!: Promise<AutoModerationEventInfo>;
  isModOrHigher: boolean = false;
  isAdminOrHigher: boolean = false;

  constructor(private route: ActivatedRoute, private auth: AuthService, private toastr: ToastrService,
     private api: ApiService, public router: Router) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid');

    this.auth.getUserProfile().subscribe((data) => {
      this.isModOrHigher = data.modGuilds.find(x => x.id === this.guildId) !== undefined || data.adminGuilds.find(x => x.id === this.guildId) !== undefined || data.isAdmin;
      this.isAdminOrHigher = data.adminGuilds.find(x => x.id === this.guildId) !== undefined || data.isAdmin;
    });

    this.guild = this.api.getSimpleData(`/discord/guilds/${this.guildId}`).toPromise();
  }
}
