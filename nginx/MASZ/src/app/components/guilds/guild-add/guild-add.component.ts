import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { Guild } from 'src/app/models/Guild';
import { GuildRole } from 'src/app/models/GuildRole';
import { MetaClientId } from 'src/app/models/MetaClientId';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';

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

  public guilds!: ContentLoading<Guild[]>;
  public searchGuilds!: string;
  public showGuilds: Guild[] = [];
  public clientId!: string;

  public selectedGuild: Guild|undefined;
  public selectedGuildDetails!: ContentLoading<Guild>;

  constructor(private _formBuilder: FormBuilder, private api: ApiService, private toastr: ToastrService, private authService: AuthService, private router: Router) { }

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
      publishModeratorInfo: ['']
    });
    
    this.reloadGuilds();
    this.api.getSimpleData('/meta/clientid').subscribe((data: MetaClientId) => {
      this.clientId = data.clientid;
    });
  }

  reloadGuilds() {
    this.searchGuilds = '';
    this.guilds = { loading: true, content: [] };
    this.showGuilds = [];

    this.api.getSimpleData('/discord/guilds').subscribe((data) => {
      this.guilds = { loading: false, content: data };
      this.showGuilds = data;
    }, () => {
      this.guilds.loading = false;
      this.toastr.error('Failed to load guilds.');
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
    this.api.getSimpleData(`/discord/guilds/${id}`).subscribe((data) => {
      this.selectedGuildDetails = { loading: false, content: data };
    }, () => {
      this.selectedGuildDetails.loading = false;
    });
  }

  invite() {
    this.selectedGuildDetails = { loading: true, content: undefined };
    var win = window.open(
      `https://discord.com/oauth2/authorize?client_id=${this.clientId}&permissions=8&scope=bot%20applications.commands&guild_id=${this.selectedGuild?.id}`, 
      "Secure Payment", "status=yes;width=150,height=400");
    if (win === null) {
      this.toastr.error("Failed to open invite window.");
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
      guildid: this.selectedGuild?.id,
      modRoles: this.modRolesGroup.value.modRoles,
      adminRoles: this.adminRolesGroup.value.adminRoles,
      mutedRoles: this.muteRolesGroup.value.muteRoles !== '' ? this.muteRolesGroup.value.muteRoles : [],
      modInternalNotificationWebhook: this.configGroup.value?.internal?.trim() != '' ? this.configGroup?.value?.internal : null,
      modPublicNotificationWebhook: this.configGroup.value?.public?.trim() != '' ? this.configGroup?.value?.public : null,
      strictModPermissionCheck: this.configGroup.value?.strictPermissionCheck !== '' ? this.configGroup.value?.strictPermissionCheck : false,
      executeWhoisOnJoin: this.configGroup.value?.executeWhoisOnJoin !== '' ? this.configGroup.value?.executeWhoisOnJoin : false,
      publishModeratorInfo: this.configGroup.value?.publishModeratorInfo !== '' ? this.configGroup.value?.publishModeratorInfo : false
    }

    this.api.postSimpleData('/guilds/', data).subscribe((data) => {
      this.toastr.success('Guild created');
      this.authService.resetCache();
      this.router.navigate(['guilds']);
    }, (error) => {
      this.toastr.error('Cannot register guild.', 'Something went wrong.');
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
