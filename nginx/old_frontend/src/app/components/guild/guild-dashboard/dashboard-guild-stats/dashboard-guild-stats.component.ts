import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { GuildStats } from 'src/app/models/GuildStats';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-dashboard-guild-stats',
  templateUrl: './dashboard-guild-stats.component.html',
  styleUrls: ['./dashboard-guild-stats.component.scss']
})
export class DashboardGuildStatsComponent implements OnInit {

  private guildId: string;
  public stats!: GuildStats;
  public loading: boolean = true;

  constructor(private route: ActivatedRoute, private api: ApiService, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid');
    this.reload();
  }

  private reload() {
    this.loading = true;
    this.stats = undefined;
    this.api.getSimpleData(`/guilds/${this.guildId}/dashboard/stats`).subscribe((data) => {
      this.stats = data;
      this.loading = false;
    }, () => {
      this.loading = false;
    });
  }
}
