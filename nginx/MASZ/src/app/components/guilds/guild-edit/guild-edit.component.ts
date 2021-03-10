import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { Guild } from 'src/app/models/Guild';
import { GuildConfig } from 'src/app/models/GuildConfig';
import { GuildRole } from 'src/app/models/GuildRole';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-guild-edit',
  templateUrl: './guild-edit.component.html',
  styleUrls: ['./guild-edit.component.css']
})
export class GuildEditComponent implements OnInit {

  public adminRolesGroup!: FormGroup;
  public modRolesGroup!: FormGroup;
  public muteRolesGroup!: FormGroup;
  public webhooksGroup!: FormGroup;
  
  public currentGuild: ContentLoading<Guild> = { loading: true, content: {} as Guild }
  public currentGuildConfig: ContentLoading<GuildConfig> = { loading: true, content: {} as GuildConfig }
  constructor(private api: ApiService, private route: ActivatedRoute, private router: Router, private toastr: ToastrService, private _formBuilder: FormBuilder) { }

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
    this.webhooksGroup = this._formBuilder.group({
      internal: ['', Validators.pattern("^https://discordapp.com/api/webhooks/.+$")],
      public: ['', Validators.pattern("^https://discordapp.com/api/webhooks/.+$")]
    });
    
    const guildId = this.route.snapshot.paramMap.get('guildid');
    this.loadGuild(guildId);
    this.loadConfig(guildId);
  }
  
  generateRoleColor(role: GuildRole): string {
    return '#' + role.color.toString(16);
  }
  
  loadGuild(id: string|null) {
    this.currentGuild = { loading: true, content: {} as Guild };
    this.api.getSimpleData(`/discord/guilds/${id}`).subscribe((data) => {
      this.currentGuild = { loading: false, content: data };
    }, () => { 
      this.currentGuild.loading = false;
      this.toastr.error('Failed to load current guild info.');
    });
  }
  
  loadConfig(id: string|null) {
    this.currentGuildConfig = { loading: true, content: {} as GuildConfig };
    this.api.getSimpleData(`/guilds/${id}`).subscribe((data: GuildConfig) => {
      this.modRolesGroup.setValue({ modRoles: data.modRoles });
      this.adminRolesGroup.setValue({ adminRoles: data.adminRoles});
      this.muteRolesGroup.setValue({ muteRoles: data.mutedRoles});
      this.webhooksGroup.setValue({ internal: data.modInternalNotificationWebhook, public: data.modPublicNotificationWebhook });
      this.currentGuildConfig = { loading: false, content: data };
    }, () => {
      this.currentGuildConfig.loading = false;
      this.toastr.error('Failed to load current guild config.');
    });
  }

  updateGuild() {
    const data = {
      modRoles: this.modRolesGroup.value.modRoles,
      adminRoles: this.adminRolesGroup.value.adminRoles,
      mutedRoles: this.muteRolesGroup.value.muteRoles !== '' ? this.muteRolesGroup.value.muteRoles : [],
      modInternalNotificationWebhook: this.webhooksGroup.value.internal.trim() != '' ? this.webhooksGroup.value.internal : null,
      modPublicNotificationWebhook: this.webhooksGroup.value.public.trim() != '' ? this.webhooksGroup.value.public : null,
    }

    this.api.putSimpleData(`/guilds/${this.currentGuild?.content?.id}`, data).subscribe((data) => {
      this.toastr.success('Guild updated.');
      this.router.navigate(['guilds']);
    }, (error) => {
      this.toastr.error('Cannot update guild.', 'Something went wrong.');
    })
  }
}
