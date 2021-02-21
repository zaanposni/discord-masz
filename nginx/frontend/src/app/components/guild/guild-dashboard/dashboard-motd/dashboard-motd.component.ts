import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { GuildMotdView } from 'src/app/models/GuildMotd';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-dashboard-motd',
  templateUrl: './dashboard-motd.component.html',
  styleUrls: ['./dashboard-motd.component.scss']
})
export class DashboardMotdComponent implements OnInit {

  public motd: GuildMotdView;
  public loading: boolean = true;
  public guildId: string;
  public updateMotdMode: boolean = false;
  public newMotd: string;
  constructor(private route: ActivatedRoute, private api: ApiService, public auth: AuthService, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid');
    this.relaod();
  }

  relaod() {
    this.loading = true;
    this.api.getSimpleData(`/guilds/${this.guildId}/motd`).subscribe((data) => {
      this.motd = data;
      this.loading = false;
    }, () => { this.loading = false; this.toastr.error('Failed to load motd.'); });
  }

  initUpdateMotd() {
    this.updateMotdMode = true;
    this.newMotd = this.motd?.motd?.message;
  }

  updateMotd() {
    this.updateMotdMode = false;
    this.loading = true;

    let data = {
      "message": this.newMotd
    };

    this.api.putSimpleData(`/guilds/${this.guildId}/motd`, data).subscribe(
      (success) => {
        this.toastr.success('Motd updated.');
        this.relaod();
      }, (error) => {
        this.loading = false;
        this.toastr.error('Failed to update motd.');
    });
  }
}
