import { HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { AutoModerationEvent } from 'src/app/models/AutoModerationEvent';
import { AutoModerationEventInfo } from 'src/app/models/AutoModerationEventInfo';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-automod-table',
  templateUrl: './automod-table.component.html',
  styleUrls: ['./automod-table.component.css']
})
export class AutomodTableComponent implements OnInit {

  guildId!: string;
  isAdminOrHigher!: Observable<boolean>;
  maxCount: number = 0;
  moderationEvents: AutoModerationEvent[] = [];
  startPage = 1;

  constructor(private api: ApiService, public router: Router, private auth: AuthService, private route: ActivatedRoute, private translator: TranslateService, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid') as string;
    this.isAdminOrHigher = this.auth.isAdminInGuild(this.guildId);
    this.api.getSimpleData(`/guilds/${this.guildId}/automoderations`).subscribe((data: AutoModerationEventInfo) => {
      this.maxCount = data.count;
      data.events.forEach((element: AutoModerationEvent) => {
        this.moderationEvents.push(element);
      });
    }, error => {
      console.error(error);
      this.toastr.error(this.translator.instant('AutoModTable.FailedToLoad'));
    });
  }

  loadMoreData(): void {
    let params = new HttpParams()
          .set('startPage', this.startPage.toString());

    this.api.getSimpleData(`/guilds/${this.guildId}/automoderations`, true, params).subscribe((data) => {
      data.events.forEach((element: AutoModerationEvent) => {
        this.moderationEvents.push(element);
      });
      this.startPage++;
    }, error => {
      console.error(error);
      this.toastr.error(this.translator.instant('AutoModTable.FailedToLoad'));
    })
  }

}
