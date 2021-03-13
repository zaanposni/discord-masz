import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { GuildMotdView } from 'src/app/models/GuildMotd';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-dashboard-motd',
  templateUrl: './dashboard-motd.component.html',
  styleUrls: ['./dashboard-motd.component.css']
})
export class DashboardMotdComponent implements OnInit {

  public motd: ContentLoading<GuildMotdView> = { loading: true, content: undefined };
  constructor(private route: ActivatedRoute, private api: ApiService, public auth: AuthService, private toastr: ToastrService) { }

  ngOnInit(): void {
    const guildId = this.route.snapshot.paramMap.get('guildid') as string;
    this.reload(guildId);
  }

  reload(guildId: string) {
    this.motd = { loading: true, content: undefined };
    this.api.getSimpleData(`/guilds/${guildId}/motd`).subscribe((data) => {
      this.motd  = { loading: false, content: data };
    }, () => {
      this.motd.loading = false;
      this.toastr.error('Failed to load motd.');
    });
  }
}
