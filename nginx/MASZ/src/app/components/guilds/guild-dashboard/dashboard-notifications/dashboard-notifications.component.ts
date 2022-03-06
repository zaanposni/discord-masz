import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { Guild } from 'src/app/models/Guild';
import { IDashboardNotifications } from 'src/app/models/IDashboardNotifications';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-dashboard-notifications',
  templateUrl: './dashboard-notifications.component.html',
  styleUrls: ['./dashboard-notifications.component.css']
})
export class DashboardNotificationsComponent implements OnInit {

  private guildId!: string;
  public anyNotification: boolean = false;
  public notifications: ContentLoading<IDashboardNotifications> = { loading: true, content: undefined };

  constructor(private route: ActivatedRoute, private api: ApiService, private toastr: ToastrService, private translator: TranslateService) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid') as string;
    this.reload();
  }

  private reload() {
    this.notifications = { loading: true, content: undefined };

    this.api.getSimpleData(`/guilds/${this.guildId}/dashboard/notifications`).subscribe((data: IDashboardNotifications) => {
      this.notifications = { loading: false, content: data };
      this.anyNotification = !data.appealConfigured;
    }, error => {
      console.error(error);
      this.notifications.loading = false;
      this.toastr.error(this.translator.instant("GuildNotifications.FailedToLoad"));
    });
  }

}
