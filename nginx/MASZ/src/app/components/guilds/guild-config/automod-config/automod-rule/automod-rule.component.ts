import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { AutomodConfig, AutoModerationAction, AutoModerationActionOptions, AutoModerationPunishmentOptions, convertToAutoModPunishment, convertToPunishmentType } from 'src/app/models/AutomodConfig';
import { AutoModRuleDefinition } from 'src/app/models/AutoModRuleDefinitions';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { Guild } from 'src/app/models/Guild';
import { GuildChannel } from 'src/app/models/GuildChannel';
import { GuildRole } from 'src/app/models/GuildRole';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-automod-rule',
  templateUrl: './automod-rule.component.html',
  styleUrls: ['./automod-rule.component.css']
})
export class AutomodRuleComponent implements OnInit {

  eventForm!: FormGroup;
  filterForm!: FormGroup;
  actionForm!: FormGroup;
  @Input() defintion!: AutoModRuleDefinition;
  @Input() guildChannels!: GuildChannel[];
  @Input() guild!: Guild;
  @Input() initialConfigs!: Promise<AutomodConfig[]>;
  public guildId!: string;
  public enableConfig: boolean = false;
  public tryingToSaveConfig: boolean = false;
  public automodActionOptions = AutoModerationActionOptions;
  public autoModerationPunishmentOptions = AutoModerationPunishmentOptions;

  constructor(private route: ActivatedRoute, private api: ApiService, private toastr: ToastrService, private _formBuilder: FormBuilder) { }

  ngOnInit(): void {    
    this.eventForm = this._formBuilder.group({
      limit: ['', this.defintion.showLimitField ? Validators.min(-1) : null]
    });
    this.filterForm = this._formBuilder.group({
      excludeRoles: [''],
      excludeChannels: ['']
    });
    this.actionForm = this._formBuilder.group({
      dmNotification: [''],
      automodAction: ['', Validators.required],
      publicNotification: [''],
      punishment: [''],
      punishmentDuration: ['']
    });

    this.actionForm.get('automodAction')?.valueChanges.subscribe(val => {
      if (val >= 2) {
          this.actionForm.controls['punishment'].setValidators([Validators.required]);
          this.actionForm.controls['punishment'].setValue(0);
          this.actionForm.controls['punishment'].updateValueAndValidity();
      } else {
          this.actionForm.controls['punishment'].clearValidators();
          this.actionForm.controls['punishment'].setValue(null);
          this.actionForm.controls['punishment'].updateValueAndValidity();
          this.actionForm.controls['punishmentDuration'].clearValidators();
          this.actionForm.controls['punishmentDuration'].setValue(null);
          this.actionForm.controls['punishmentDuration'].updateValueAndValidity();
      }
    });

    this.actionForm.get('punishment')?.valueChanges.subscribe(val => {
      if (val >= 4) {
          this.actionForm.controls['punishmentDuration'].setValidators([Validators.required, Validators.min(1)]);
          this.actionForm.controls['punishmentDuration'].updateValueAndValidity();
      } else {
          this.actionForm.controls['punishmentDuration'].clearValidators();
          this.actionForm.controls['punishmentDuration'].setValue(null);
          this.actionForm.controls['punishmentDuration'].updateValueAndValidity();
      }
    });

    this.guildId = this.route.snapshot.paramMap.get('guildid') as string;
    this.initialConfigs.then((data: AutomodConfig[]) => {  
      // if type in initial loaded configs
      if (data.filter(x => x.autoModerationType == this.defintion.type).length) {
        this.enableConfig = true;
        this.applyConfig(data.filter(x => x.autoModerationType == this.defintion.type)[0]);
      } else {        
        this.enableConfig = false;
      }
    })
  }

  generateRoleColor(role: GuildRole): string {
    return '#' + role.color.toString(16);
  }

  reload() {
    this.enableConfig = false;
    this.api.getSimpleData(`/guilds/${this.guildId}/automoderationconfig/${this.defintion.type}`).subscribe((data) => {
      this.enableConfig = true;
      this.applyConfig(data);
    })
  }

  applyConfig(config: AutomodConfig) {
    if (this.defintion.showLimitField) {
      this.eventForm.setValue({ limit: config.limit });
    }
    
    this.filterForm.setValue({ excludeRoles: config.ignoreRoles, excludeChannels: config.ignoreChannels });

    this.actionForm.setValue({
      dmNotification: config.sendDmNotification,
      publicNotification: config.sendPublicNotification,
      automodAction: config.autoModerationAction,
      punishment: convertToAutoModPunishment(config.punishmentType, config.punishmentDurationMinutes),
      punishmentDuration: config.punishmentDurationMinutes
    });
  }

  onRuleToggle(event: any) {
    if (!event) {
      this.deleteConfig();
    }
  }

  deleteConfig() {
    this.api.deleteData(`/guilds/${this.guildId}/automoderationconfig/${this.defintion.type}`).subscribe((data) => {
      this.toastr.success("Config deleted.");
      this.reload();
    }, (error) => {
      console.log('Failed to delete config.');
      this.reload();
    });
  }

  saveConfig() {
    this.tryingToSaveConfig = true;
    const data = {
      "AutoModerationType": this.defintion.type,
      "AutoModerationAction": this.actionForm.value.automodAction,
      "PunishmentType": convertToPunishmentType(this.actionForm.value.punishment),
      "PunishmentDurationMinutes": this.actionForm.value.punishmentDuration !== "" ? this.actionForm.value.punishmentDuration : null,
      "IgnoreChannels": this.filterForm.value.excludeChannels !== "" ? this.filterForm.value.excludeChannels : [],
      "IgnoreRoles": this.filterForm.value.excludeRoles !== "" ? this.filterForm.value.excludeRoles : [],
      "TimeLimitMinutes": null as any,
      "Limit": this.eventForm.value.limit !== "" ? this.eventForm.value.limit : null,
      "SendDmNotification": this.actionForm.value.dmNotification !== "" ? this.actionForm.value.dmNotification : false,
      "SendPublicNotification": this.actionForm.value.publicNotification !== "" ? this.actionForm.value.publicNotification : false
    }
    
    this.api.putSimpleData(`/guilds/${this.guildId}/automoderationconfig`, data).subscribe((data) => {
      this.tryingToSaveConfig = false;
      this.toastr.success("Saved config.");
      this.reload();
    }, (error) => {
      this.tryingToSaveConfig = false;
      this.toastr.error('Failed to update config.')
    });    

  }
}
