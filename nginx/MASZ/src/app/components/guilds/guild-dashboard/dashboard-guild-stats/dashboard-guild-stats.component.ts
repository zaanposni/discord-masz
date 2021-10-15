import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { GuildStats } from 'src/app/models/GuildStats';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-dashboard-guild-stats',
  templateUrl: './dashboard-guild-stats.component.html',
  styleUrls: ['./dashboard-guild-stats.component.css']
})
export class DashboardGuildStatsComponent implements OnInit {

  public stats: ContentLoading<GuildStats> = { loading: true, content: undefined };

  constructor(private route: ActivatedRoute, private api: ApiService, private toastr: ToastrService, private translator: TranslateService) { }

  ngOnInit(): void {
    const guildId = this.route.snapshot.paramMap.get('guildid');
    this.initialize(guildId as string);
  }

  initialize(guildId: string) {
    this.stats = { loading: true, content: undefined };
      this.api.getSimpleData(`/guilds/${guildId}/dashboard/stats`).subscribe(data => {
        this.stats.content = data;
        this.stats.loading = false;
      }, error => {
        console.error(error);
        this.stats.loading = false;
        this.toastr.error(this.translator.instant('GuildStats.FailedToLoad'));
      });
  }
}
