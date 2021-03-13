import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Guild } from 'src/app/models/Guild';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-dashboard-guildinfo',
  templateUrl: './dashboard-guildinfo.component.html',
  styleUrls: ['./dashboard-guildinfo.component.scss']
})
export class DashboardGuildinfoComponent implements OnInit {

  private guildId: string;
  public guild: Guild;
  public loading: boolean = true;

  constructor(private route: ActivatedRoute, private api: ApiService, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid');
    this.reload();
  }

  private reload() {
    this.loading = true;

    this.api.getSimpleData(`/discord/guilds/${this.guildId}`).subscribe((data) => {
      this.guild = data;
      this.loading = false;
    }, () => { this.loading = false; this.toastr.error('Failed to load guild info.'); });
  }
}
