import { HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { APIEnumTypes } from 'src/app/models/APIEmumTypes';
import { APIEnum } from 'src/app/models/APIEnum';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { Guild } from 'src/app/models/Guild';
import { GuildRole } from 'src/app/models/GuildRole';
import { IApplicationInfo } from 'src/app/models/IApplicationInfo';
import { ApiService } from 'src/app/services/api.service';
import { ApplicationInfoService } from 'src/app/services/application-info.service';
import { AuthService } from 'src/app/services/auth.service';
import { EnumManagerService } from 'src/app/services/enum-manager.service';

@Component({
  selector: 'app-guild-add',
  templateUrl: './guild-add.component.html',
  styleUrls: ['./guild-add.component.css']
})
export class GuildAddComponent implements OnInit {

  public adminRolesGroup!: FormGroup;
  public modRolesGroup!: FormGroup;
  public muteRolesGroup!: FormGroup;
  public configGroup!: FormGroup;
  public queryGroup!: FormGroup;

  public guilds!: ContentLoading<Guild[]>;
  public searchGuilds!: string;
  public showGuilds: Guild[] = [];
  public clientId!: string;

  public selectedGuild: Guild|undefined;
  public selectedGuildDetails!: ContentLoading<Guild>;

  public allLanguages: APIEnum[] = [];

  constructor(private _formBuilder: FormBuilder, private api: ApiService, private toastr: ToastrService, private authService: AuthService, private router: Router, private applicationInfoService: ApplicationInfoService, private translator: TranslateService, private enumManager: EnumManagerService) { }

  ngOnInit(): void {

    this.adminRolesGroup = this._formBuilder.group({
      adminRoles: ['', Validators.required]
    });
    this.modRolesGroup = this._formBuilder.group({
      modRoles: ['', Validators.required]
    });
    this.muteRolesGroup = this._formBuilder.group({
      muteRoles: ['']
    });
    this.configGroup = this._formBuilder.group({
      internal: ['', Validators.pattern("^https://discord(app)?\.com/api/webhooks/.+$")],
      public: ['', Validators.pattern("^https://discord(app)?\.com/api/webhooks/.+$")],
      strictPermissionCheck: [''],
      executeWhoisOnJoin: [''],
      publishModeratorInfo: [''],
      preferredLanguage: [''],
    });
    this.queryGroup = this._formBuilder.group({
      importExistingBans: [''],
    });

    this.applicationInfoService.currentApplicationInfo.subscribe((data: IApplicationInfo) => {
      this.clientId = data.id;
    });

    this.loadLanguages();
    this.reloadGuilds();
  }

  reloadGuilds() {
    this.searchGuilds = '';
    this.guilds = { loading: true, content: [] };
    this.showGuilds = [];

    this.api.getSimpleData('/discord/guilds').subscribe(data => {
      this.guilds = { loading: false, content: data };
      this.showGuilds = data;
    }, error => {
      console.error(error);
      this.guilds.loading = false;
      this.toastr.error(this.translator.instant('GuildDialog.FailedToLoadGuilds'));
    });
  }

  loadLanguages() {
    this.enumManager.getEnum(APIEnumTypes.LANGUAGE).subscribe((data: APIEnum[]) => {
      this.allLanguages = data;
    });
  }

  onSearch() {
    if (this.searchGuilds.trim() === '') {
      this.showGuilds = this.guilds.content as Guild[];
    }
    this.showGuilds = this.guilds.content?.filter(x => x.name.toLowerCase().includes(this.searchGuilds.toLowerCase()) || x.id.includes(this.searchGuilds)) as Guild[];
  }

  resetSearch() {
    this.searchGuilds = '';
    this.onSearch();
  }

  generateRoleColor(role: GuildRole): string {
    return '#' + role.color.toString(16);
  }

  selectGuild(id: string) {
    this.searchGuilds = '';
    this.selectedGuild = this.guilds.content?.find(x => x.id === id) as Guild;
    this.selectedGuildDetails = { loading: true, content: undefined };
    this.api.getSimpleData(`/discord/guilds/${id}`).subscribe((data: Guild) => {
      data.roles = data.roles.sort((a, b) => (a.position < b.position) ? 1 : -1);
      this.selectedGuildDetails = { loading: false, content: data };
    }, error => {
      console.error(error);
      this.selectedGuildDetails.loading = false;
    });
  }

  invite() {
    this.selectedGuildDetails = { loading: true, content: undefined };
    var win = window.open(
      `https://discord.com/oauth2/authorize?client_id=${this.clientId}&permissions=8&scope=bot%20applications.commands&guild_id=${this.selectedGuild?.id}`,
      "Secure Payment", "status=yes;width=150,height=400");
    if (win === null) {
      this.toastr.error(this.translator.instant('GuildDialog.FailedInviteWindow'));
      return;
    }
    var timer = setInterval(function(callback: any, id: string, context: any) {
      if (win?.closed) {
          clearInterval(timer);
          callback.bind(context, id)();
      }
    }, 500, this.selectGuild, this.selectedGuild?.id, this);
  }

  registerGuild() {
    const data = {
      guildid:                        this.selectedGuild?.id,
      modRoles:                       this.modRolesGroup.value.modRoles,
      adminRoles:                     this.adminRolesGroup.value.adminRoles,
      mutedRoles:                     this.muteRolesGroup.value.muteRoles           != '' ? this.muteRolesGroup.value.muteRoles                    : [],
      modInternalNotificationWebhook: this.configGroup.value?.internal?.trim()      != '' ? this.configGroup?.value?.internal                      : null,
      modPublicNotificationWebhook:   this.configGroup.value?.public?.trim()        != '' ? this.configGroup?.value?.public                        : null,
      strictModPermissionCheck:       this.configGroup.value?.strictPermissionCheck != '' ? this.configGroup.value?.strictPermissionCheck ?? false : false,
      executeWhoisOnJoin:             this.configGroup.value?.executeWhoisOnJoin    != '' ? this.configGroup.value?.executeWhoisOnJoin    ?? false : false,
      publishModeratorInfo:           this.configGroup.value?.publishModeratorInfo  != '' ? this.configGroup.value?.publishModeratorInfo  ?? false : false,
      preferredLanguage:              this.configGroup.value?.preferredLanguage     != '' ? this.configGroup.value?.preferredLanguage     ?? null  : null
    };
    let params = new HttpParams()
                      .set("importExistingBans", this.queryGroup.value?.importExistingBans ? 'true' : 'false');
    this.api.postSimpleData('/guilds/', data, params).subscribe(() => {
      this.toastr.success(this.translator.instant('GuildDialog.GuildCreated'));
      this.authService.resetCache();
      this.router.navigate(['guilds']);
    }, error => {
      console.error(error);
      this.toastr.error(this.translator.instant('GuildDialog.FailedToRegisterGuild'));
    })
  }

  unselectGuild() {
    this.selectedGuild = undefined;
    this.selectedGuildDetails = { loading: true, content: undefined };
    this.reloadGuilds();
    this.modRolesGroup.reset();
    this.adminRolesGroup.reset();
    this.muteRolesGroup.reset();
    this.configGroup.reset();
  }
}
