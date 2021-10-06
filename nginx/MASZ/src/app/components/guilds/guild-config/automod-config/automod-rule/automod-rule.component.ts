import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { APIEnumTypes } from 'src/app/models/APIEmumTypes';
import { APIEnum } from 'src/app/models/APIEnum';
import { AutoModerationConfig } from 'src/app/models/AutoModerationConfig';
import { AutoModRuleDefinition } from 'src/app/models/AutoModRuleDefinitions';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { Guild } from 'src/app/models/Guild';
import { GuildChannel } from 'src/app/models/GuildChannel';
import { GuildRole } from 'src/app/models/GuildRole';
import { ApiService } from 'src/app/services/api.service';
import { EnumManagerService } from 'src/app/services/enum-manager.service';

@Component({
  selector: 'app-automod-rule',
  templateUrl: './automod-rule.component.html',
  styleUrls: ['./automod-rule.component.css']
})
export class AutomodRuleComponent implements OnInit {

  eventForm!: FormGroup;
  filterForm!: FormGroup;
  actionForm!: FormGroup;
  @Input() definition!: AutoModRuleDefinition;
  @Input() guildChannels!: GuildChannel[];
  @Input() guild!: Guild;
  @Input() initialConfigs!: Promise<AutoModerationConfig[]>;
  public guildId!: string;
  public enableConfig: boolean = false;
  public tryingToSaveConfig: boolean = false;
  public automodActionOptions: ContentLoading<APIEnum[]> = { loading: true, content: [] };
  public punishmentTypes: ContentLoading<APIEnum[]> = { loading: true, content: [] };

  public initRowsCustomWords = 1;

  constructor(private route: ActivatedRoute, private api: ApiService, private toastr: ToastrService, private _formBuilder: FormBuilder, private enumManager: EnumManagerService, private translator: TranslateService) { }

  ngOnInit(): void {
    this.eventForm = this._formBuilder.group({
      limit: ['', this.definition.showLimitField ? Validators.min(-1) : null],
      timeLimit: ['', this.definition.showTimeLimitField ? Validators.min(-1) : null],
      customWord: ['', this.definition.showCustomField ? Validators.required : null]
    });
    this.filterForm = this._formBuilder.group({
      excludeRoles: [''],
      excludeChannels: ['']
    });
    this.actionForm = this._formBuilder.group({
      dmNotification: [''],
      automodAction: ['', Validators.required],
      publicNotification: [''],
      punishmentType: [''],
      punishmentDuration: ['']
    });

    this.actionForm.get('automodAction')?.valueChanges.subscribe(val => {
      if (val >= 2) {
          this.actionForm.controls['punishmentType'].setValidators([Validators.required]);
          this.actionForm.controls['punishmentType'].setValue(0);
          this.actionForm.controls['punishmentType'].updateValueAndValidity();
      } else {
          this.actionForm.controls['punishmentType'].clearValidators();
          this.actionForm.controls['punishmentType'].setValue(null);
          this.actionForm.controls['punishmentType'].updateValueAndValidity();
          this.actionForm.controls['punishmentDuration'].clearValidators();
          this.actionForm.controls['punishmentDuration'].setValue(null);
          this.actionForm.controls['punishmentDuration'].updateValueAndValidity();
      }
    });

    this.guildId = this.route.snapshot.paramMap.get('guildid') as string;
    this.initialConfigs.then((data: AutoModerationConfig[]) => {
      // if type in initial loaded configs
      if (data.filter(x => x.autoModerationType == this.definition.type).length) {
        this.enableConfig = true;
        this.applyConfig(data.filter(x => x.autoModerationType == this.definition.type)[0]);
      } else {
        this.enableConfig = false;
      }
    });

    this.enumManager.getEnum(APIEnumTypes.AUTOMODACTION).subscribe((data) => {
      this.automodActionOptions.loading = false;
      this.automodActionOptions.content = data;
    }, (error) => {
      console.error(error);
      this.automodActionOptions.loading = false;
    });

    this.enumManager.getEnum(APIEnumTypes.PUNISHMENT).subscribe((data) => {
      this.punishmentTypes.loading = false;
      this.punishmentTypes.content = data;
    }, (error) => {
      console.error(error);
      this.punishmentTypes.loading = false;
    });
  }

  generateRoleColor(role: GuildRole): string {
    return '#' + role.color.toString(16);
  }

  reload() {
    this.enableConfig = false;
    this.api.getSimpleData(`/guilds/${this.guildId}/automoderationconfig/${this.definition.type}`).subscribe((data) => {
      this.enableConfig = true;
      this.applyConfig(data);
    });
  }

  applyConfig(config: AutoModerationConfig) {
    if (this.definition.showLimitField) {
      this.eventForm.setValue({
        limit: config.limit,
        timeLimit: this.definition.showTimeLimitField ? config.timeLimitMinutes : '',
        customWord: this.definition.showCustomField ? config.customWordFilter : ''
      });
    }
    if (config.customWordFilter) {
      this.initRowsCustomWords = Math.min(config.customWordFilter.split(/\r\n|\r|\n/).length, 15);
    }

    this.filterForm.setValue({ excludeRoles: config.ignoreRoles, excludeChannels: config.ignoreChannels });

    this.actionForm.setValue({
      dmNotification: config.sendDmNotification,
      publicNotification: config.sendPublicNotification,
      automodAction: config.autoModerationAction,
      punishmentType: config.punishmentType,
      punishmentDuration: config.punishmentDurationMinutes
    });
  }

  onRuleToggle(event: any) {
    if (!event) {
      this.deleteConfig();
    }
  }

  deleteConfig() {
    this.api.deleteData(`/guilds/${this.guildId}/automoderationconfig/${this.definition.type}`).subscribe(() => {
      this.toastr.success(this.translator.instant('AutomodConfig.ConfigDeleted'));
      this.reload();
    }, error => {
      console.error(error);
      if (error?.error?.status !== 404) {
        this.toastr.error(this.translator.instant('AutomodConfig.FailedToDeleteConfig'));
      }
      this.reload();
    });
  }

  saveConfig() {
    this.tryingToSaveConfig = true;
    const data = {
      "AutoModerationType": this.definition.type,
      "AutoModerationAction": this.actionForm.value.automodAction,
      "PunishmentType": this.actionForm.value.punishmentType,
      "PunishmentDurationMinutes": this.actionForm.value.punishmentDuration !== "" ? this.actionForm.value.punishmentDuration : null,
      "IgnoreChannels": this.filterForm.value.excludeChannels !== "" ? this.filterForm.value.excludeChannels : [],
      "IgnoreRoles": this.filterForm.value.excludeRoles !== "" ? this.filterForm.value.excludeRoles : [],
      "TimeLimitMinutes": this.eventForm.value.timeLimit !== "" ? this.eventForm.value.timeLimit : null,
      "CustomWordFilter": this.eventForm.value.customWord !== "" ? this.eventForm.value.customWord : null,
      "Limit": this.eventForm.value.limit !== "" ? this.eventForm.value.limit : null,
      "SendDmNotification": this.actionForm.value.dmNotification !== "" ? this.actionForm.value.dmNotification : false,
      "SendPublicNotification": this.actionForm.value.publicNotification !== "" ? this.actionForm.value.publicNotification : false
    }

    this.api.putSimpleData(`/guilds/${this.guildId}/automoderationconfig`, data).subscribe(() => {
      this.tryingToSaveConfig = false;
      this.toastr.success(this.translator.instant('AutomodConfig.SavedConfig'));
      this.reload();
    }, error => {
      console.error(error);
      this.tryingToSaveConfig = false;
      this.toastr.error(this.translator.instant('AutomodConfig.FailedToSaveConfig'))
    });

  }
}
