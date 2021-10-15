import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
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
  public motdParams = { name: '' };

  constructor(private route: ActivatedRoute, private api: ApiService, public auth: AuthService, private toastr: ToastrService, private translator: TranslateService) { }

  ngOnInit(): void {
    const guildId = this.route.snapshot.paramMap.get('guildid') as string;
    this.reload(guildId);
  }

  reload(guildId: string) {
    this.motd = { loading: true, content: undefined };
    this.api.getSimpleData(`/guilds/${guildId}/motd`).subscribe((data: GuildMotdView) => {
      this.motd  = { loading: false, content: data };
      this.motdParams.name = `${data.creator?.username}#${data.creator?.discriminator}`;
    }, error => {
      this.motd.loading = false;
      if (error?.error?.status !== 404 && error?.status !== 404) {
        console.error(error);
        this.toastr.error(this.translator.instant('DashboardMotd.FailedToLoad'));
      }
    });
  }
}
