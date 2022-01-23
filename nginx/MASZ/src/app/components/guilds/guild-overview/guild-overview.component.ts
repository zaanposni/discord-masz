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

  private modSub: any;
  private adminSub: any;
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
      this.modSub?.unsubscribe();
      this.adminSub?.unsubscribe();
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
    this.modSub = this.auth.isModInGuild(guildId).subscribe((data: boolean) => {
      this.isModOrHigher = data;
      if (data) {
        this.tabs.unshift({ component: 'dashboard', icon: 'dashboard' });
        this.tabs.push({ component: 'usernote', icon: 'badge' });
        this.tabs.push({ component: 'usermap', icon: 'people' });
        this.tabs.push({ component: 'messages', icon: 'chat' });
        setTimeout(() => {
          this.selectedTab.setValue(-1);
        }, 200);
      }
    });
    this.adminSub = this.auth.isAdminInGuild(guildId).subscribe((data: boolean) => {
      this.isAdminOrHigher = data;
      if (data) {
        this.tabs.push({ component: 'config', icon: 'settings' });
      }
    });
  }
}
