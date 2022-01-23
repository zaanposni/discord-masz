import { Component, Input, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatStepper } from '@angular/material/stepper';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { APIEnumTypes } from 'src/app/models/APIEmumTypes';
import { Guild } from 'src/app/models/Guild';
import { GuildChannel } from 'src/app/models/GuildChannel';
import { GuildRole } from 'src/app/models/GuildRole';
import { IGuildAuditLogConfig } from 'src/app/models/IGuildAuditLogConfig';
import { IGuildAuditLogRuleDefinition } from 'src/app/models/IGuildAuditLogRuleDefinition';
import { ApiService } from 'src/app/services/api.service';
import { EnumManagerService } from 'src/app/services/enum-manager.service';

@Component({
  selector: 'app-auditlog-config-rule',
  templateUrl: './auditlog-config-rule.component.html',
  styleUrls: ['./auditlog-config-rule.component.css']
})
export class AuditlogConfigRuleComponent implements OnInit {

  configForm!: FormGroup;
  @Input() definition!: IGuildAuditLogRuleDefinition;
  @Input() guildChannels!: GuildChannel[];
  @Input() guild!: Guild;
  @Input() initialConfigs!: Promise<IGuildAuditLogConfig[]>;
  public guildId!: string;
  public enableConfig: boolean = false;
  public tryingToSaveConfig: boolean = false;
  public initStuff: boolean = true;

  public initRowsCustomWords = 1;

  constructor(private route: ActivatedRoute, private api: ApiService, private toastr: ToastrService, private _formBuilder: FormBuilder, private translator: TranslateService) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid') as string;

    this.configForm = this._formBuilder.group({
      channel: [''],
      pingRoles: ['']
    });

    this.configForm.valueChanges.subscribe(() => {
      if (!this.tryingToSaveConfig && !this.initStuff) {
        this.saveConfig();
      }
    });

    this.initialConfigs.then((data: IGuildAuditLogConfig[]) => {
      // if type in initial loaded configs
      if (data.filter(x => x.guildAuditLogEvent == this.definition.type).length) {
        this.enableConfig = true;
        this.applyConfig(data.filter(x => x.guildAuditLogEvent == this.definition.type)[0]);
        this.initStuff = false;
      } else {
        this.enableConfig = false;
        this.initStuff = false;
      }
    });
  }

  generateRoleColor(role: GuildRole): string {
    return '#' + role.color.toString(16);
  }

  reload() {
    this.enableConfig = false;
    this.api.getSimpleData(`/guilds/${this.guildId}/auditlog/${this.definition.type}`).subscribe((data) => {
      this.enableConfig = true;
      this.applyConfig(data);
    });
  }

  applyConfig(config: IGuildAuditLogConfig) {
    this.configForm.setValue({
      channel: config.channelId,
      pingRoles: config.pingRoles,
    });
  }

  onRuleToggle(event: any) {
    if (!event) {
      this.deleteConfig();
    }
  }

  deleteConfig() {
    this.tryingToSaveConfig = true;
    this.api.deleteData(`/guilds/${this.guildId}/auditlog/${this.definition.type}`).subscribe(() => {
      this.toastr.success(this.translator.instant('GuildAuditLogConfig.ConfigDeleted'));
      this.configForm.setValue({
        channel: null,
        pingRoles: null,
      });
      this.tryingToSaveConfig = false;
    }, error => {
      console.error(error);
      if (error?.error?.status !== 404 && error?.status !== 404) {
        this.toastr.error(this.translator.instant('GuildAuditLogConfig.FailedToDeleteConfig'));
      }
      this.tryingToSaveConfig = false;
    });
  }

  saveConfig() {
    this.tryingToSaveConfig = true;
    const data = {
      "GuildAuditLogEvent": this.definition.type,
      "ChannelId": this.configForm.value.channel ? this.configForm.value.channel : 0,
      "PingRoles": this.configForm.value.pingRoles ? this.configForm.value.pingRoles : [],
    }

    this.api.putSimpleData(`/guilds/${this.guildId}/auditlog`, data).subscribe((data) => {
      this.toastr.success(this.translator.instant('GuildAuditLogConfig.SavedConfig'));
      this.applyConfig(data);
      this.tryingToSaveConfig = false;
    }, error => {
      console.error(error);
      this.tryingToSaveConfig = false;
      this.toastr.error(this.translator.instant('GuildAuditLogConfig.FailedToSaveConfig'))
    });
  }
}
