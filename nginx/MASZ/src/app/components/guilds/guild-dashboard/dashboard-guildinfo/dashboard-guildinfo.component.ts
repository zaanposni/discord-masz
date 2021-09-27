import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { Guild } from 'src/app/models/Guild';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-dashboard-guildinfo',
  templateUrl: './dashboard-guildinfo.component.html',
  styleUrls: ['./dashboard-guildinfo.component.css']
})
export class DashboardGuildinfoComponent implements OnInit {

  private guildId!: string;
  public guild: ContentLoading<Guild> = { loading: true, content: undefined };

  constructor(private route: ActivatedRoute, private api: ApiService, private toastr: ToastrService, private translator: TranslateService) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid') as string;
    this.reload();
  }

  private reload() {
    this.guild = { loading: true, content: undefined };

    this.api.getSimpleData(`/discord/guilds/${this.guildId}`).subscribe(data => {
      this.guild = { loading: false, content: data };
    }, error => {
      console.error(error);
      this.guild.loading = false;
      this.toastr.error(this.translator.instant("GuildInfoCard.FailedToLoad"));
    });
  }

}
