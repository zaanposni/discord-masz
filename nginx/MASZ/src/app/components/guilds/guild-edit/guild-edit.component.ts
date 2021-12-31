import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { APIEnumTypes } from 'src/app/models/APIEmumTypes';
import { APIEnum } from 'src/app/models/APIEnum';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { Guild } from 'src/app/models/Guild';
import { GuildConfig } from 'src/app/models/GuildConfig';
import { GuildRole } from 'src/app/models/GuildRole';
import { ApiService } from 'src/app/services/api.service';
import { EnumManagerService } from 'src/app/services/enum-manager.service';

@Component({
  selector: 'app-guild-edit',
  templateUrl: './guild-edit.component.html',
  styleUrls: ['./guild-edit.component.css']
})
export class GuildEditComponent implements OnInit {

  public adminRolesGroup!: FormGroup;
  public modRolesGroup!: FormGroup;
  public muteRolesGroup!: FormGroup;
  public configGroup!: FormGroup;

  public allLanguages: APIEnum[] = [];

  public currentGuild: ContentLoading<Guild> = { loading: true, content: {} as Guild }
  public currentGuildConfig: ContentLoading<GuildConfig> = { loading: true, content: {} as GuildConfig }
  constructor(private api: ApiService, private route: ActivatedRoute, private router: Router, private toastr: ToastrService, private _formBuilder: FormBuilder, private translator: TranslateService, private enumManager: EnumManagerService) { }

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
      preferredLanguage: ['']
    });

      this.route.paramMap.subscribe(params => {
        const guildId = params.get("guildid");
        this.loadLanguages();
        this.loadGuild(guildId);
        this.loadConfig(guildId);
      });
  }

  generateRoleColor(role: GuildRole): string {
    return '#' + role.color.toString(16);
  }

  loadGuild(id: string|null) {
    this.currentGuild = { loading: true, content: {} as Guild };
    this.api.getSimpleData(`/discord/guilds/${id}`).subscribe((data: Guild) => {
      data.roles = data.roles.sort((a, b) => (a.position < b.position) ? 1 : -1);
      this.currentGuild = { loading: false, content: data };
    }, error => {
      console.error(error);
      this.currentGuild.loading = false;
      this.toastr.error(this.translator.instant('GuildDialog.FailedToLoadCurrentGuild'));
    });
  }

  loadLanguages() {
    this.enumManager.getEnum(APIEnumTypes.LANGUAGE).subscribe((data: APIEnum[]) => {
      this.allLanguages = data;
    });
  }

  loadConfig(id: string|null) {
    this.currentGuildConfig = { loading: true, content: {} as GuildConfig };
    this.api.getSimpleData(`/guilds/${id}`).subscribe((data: GuildConfig) => {
      this.modRolesGroup.setValue({ modRoles: data.modRoles });
      this.adminRolesGroup.setValue({ adminRoles: data.adminRoles});
      this.muteRolesGroup.setValue({ muteRoles: data.mutedRoles});
      this.configGroup.setValue({
        internal: data.modInternalNotificationWebhook,
        public: data.modPublicNotificationWebhook,
        strictPermissionCheck: data.strictModPermissionCheck,
        executeWhoisOnJoin: data.executeWhoisOnJoin,
        publishModeratorInfo: data.publishModeratorInfo,
        preferredLanguage: data.preferredLanguage
      });
      this.currentGuildConfig = { loading: false, content: data };
    }, error => {
      console.error(error);
      this.currentGuildConfig.loading = false;
      this.toastr.error(this.translator.instant('GuildDialog.FailedToLoadCurrentGuild'));
    });
  }

  updateGuild() {
    const data = {
      modRoles: this.modRolesGroup.value.modRoles,
      adminRoles: this.adminRolesGroup.value.adminRoles,
      mutedRoles: this.muteRolesGroup.value.muteRoles != '' ? this.muteRolesGroup.value.muteRoles : [],
      modInternalNotificationWebhook: this.configGroup.value?.internal?.trim() ? this.configGroup?.value?.internal : null,
      modPublicNotificationWebhook: this.configGroup.value?.public?.trim() ? this.configGroup?.value?.public : null,
      strictModPermissionCheck: (this.configGroup.value?.strictPermissionCheck != '' ? this.configGroup.value?.strictPermissionCheck : false) ?? false,
      executeWhoisOnJoin: (this.configGroup.value?.executeWhoisOnJoin != '' ? this.configGroup.value?.executeWhoisOnJoin : false) ?? false,
      publishModeratorInfo: (this.configGroup.value?.publishModeratorInfo != '' ? this.configGroup.value?.publishModeratorInfo : false) ?? false,
      preferredLanguage: this.configGroup.value?.preferredLanguage != '' ? this.configGroup.value?.preferredLanguage : 0
    }

    this.api.putSimpleData(`/guilds/${this.currentGuild?.content?.id}`, data).subscribe(() => {
      this.toastr.success(this.translator.instant('GuildDialog.GuildUpdated'));
      this.router.navigate(['guilds']);
    }, error => {
      console.error(error);
      this.toastr.error(this.translator.instant('GuildDialog.FailedToUpdateGuild'));
    })
  }
}
