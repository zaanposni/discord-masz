import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { GuildMotd, GuildMotdView } from 'src/app/models/GuildMotd';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-motd-config',
  templateUrl: './motd-config.component.html',
  styleUrls: ['./motd-config.component.css']
})
export class MotdConfigComponent implements OnInit {

  timeout: any = null;
  private guildId!: string;
  public motd: ContentLoading<GuildMotdView> = { loading: true, content: undefined };
  constructor(private route: ActivatedRoute, private api: ApiService, private toastr: ToastrService, private translator: TranslateService) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid') as string;
    this.reload();
  }

  private reload() {
    this.motd = { loading: true, content: undefined };
    this.api.getSimpleData(`/guilds/${this.guildId}/motd`).subscribe((data: GuildMotdView) => {
      this.motd.content = data;
      this.motd.loading = false;
    }, error => {
      this.motd.loading = false;
      if (error?.error?.status === 404 || error?.status === 404) {
        this.motd.content = {
          motd: {
            guildId: this.guildId,
            message: "Example message",
            showMotd: false
          } as any,
          creator: undefined
        };
      } else {
        console.error(error);
        this.toastr.error(this.translator.instant('GuildMotd.FailedToLoad'));
      }
    });
  }

  onChange(event: any) {
    clearTimeout(this.timeout);
    var $this = this;
    this.timeout = setTimeout(function () {
      if (event.keyCode != 13) {
        $this.updateMotd();
      }
    }, 500);
  }

  onToggle(event: any) {
    var $this = this;
    setTimeout(function () { $this.updateMotd() }, 100);  // wait a bit because angular sux
  }

  public updateMotd() {
    let data = {
      "message": this.motd.content?.motd.message,
      "showMotd": this.motd.content?.motd.showMotd
    };

    if (data.message?.trim()?.length == 0) {
      data.message = "Placeholder";
    }

    this.api.putSimpleData(`/guilds/${this.guildId}/motd`, data).subscribe(() => {
        this.toastr.success(this.translator.instant('GuildMotd.Saved'));
        this.reload();
      }, error => {
        console.error(error);
        this.toastr.error(this.translator.instant('GuildMotd.FailedToSave'));
    });
  }

}
