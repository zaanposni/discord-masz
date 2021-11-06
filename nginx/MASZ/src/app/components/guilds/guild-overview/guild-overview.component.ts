import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { IDashboardTabs } from 'src/app/models/IDashboardTabs';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-guild-overview',
  templateUrl: './guild-overview.component.html',
  styleUrls: ['./guild-overview.component.css']
})
export class GuildOverviewComponent implements OnInit {

  public tabs: IDashboardTabs[] = [
    {
      "icon": "list",
      "component": "cases"
    },
    {
      "icon": "bolt",
      "component": "automods"
    }
  ]
  public isModOrHigher: boolean = false;
  public isAdminOrHigher: boolean = false;
  public selectedTab = new FormControl(0);

  constructor(private auth: AuthService, private route: ActivatedRoute, private translator: TranslateService) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe((data) => {
      this.initialize(data.get('guildid') as string);
    });
  }

  initialize(guildId: string) {
    this.tabs = [
      {
        "icon": "list",
        "component": "cases"
      },
      {
        "icon": "bolt",
        "component": "automods"
      }
    ];
    this.auth.isModInGuild(guildId).subscribe((data) => {
      this.isModOrHigher = data;
      if (data) {
        this.tabs.unshift({ component: 'dashboard', icon: 'dashboard' });
        this.tabs.push({ component: 'usernote', icon: 'badge' });
        this.tabs.push({ component: 'usermap', icon: 'people' });
        this.tabs.push({ component: 'bin', icon: 'delete_forever' });
        setTimeout(() => {
          this.selectedTab.setValue(-1);
        }, 200);
      }
    });
    this.auth.isAdminInGuild(guildId).subscribe((data) => {
      this.isAdminOrHigher = data;
      if (data) {
        this.tabs.push({ component: 'config', icon: 'settings' });
      }
    });
  }

}
