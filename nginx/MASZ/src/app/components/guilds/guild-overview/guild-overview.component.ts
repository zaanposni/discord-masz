import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-guild-overview',
  templateUrl: './guild-overview.component.html',
  styleUrls: ['./guild-overview.component.css']
})
export class GuildOverviewComponent implements OnInit {

  public tabs = [
    {
      "label": "Cases",
      "icon": "list",
      "component": "cases"
    },
    {
      "label": "Automoderations",
      "icon": "bolt",
      "component": "automods"
    }
  ]
  public isModOrHigher: boolean = false;
  public isAdminOrHigher: boolean = false;
  selectedTab = new FormControl(0);

  constructor(private auth: AuthService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe((data) => {
      this.initialize(data.get('guildid') as string);
    });
  }

  initialize(guildId: string) {
    this.tabs = [
      
      {
        "label": "Cases",
        "icon": "list",
        "component": "cases"
      },
      {
        "label": "Automoderations",
        "icon": "bolt",
        "component": "automods"
      }
    ];
    this.auth.isModInGuild(guildId).subscribe((data) => {
      this.isModOrHigher = data;
      if (data) {
        this.tabs.unshift({ component: 'dashboard', icon: 'dashboard', label: 'Dashboard' });
        this.selectedTab.setValue(0);
      }
    });
    this.auth.isAdminInGuild(guildId).subscribe((data) => {
      this.isAdminOrHigher = data;
      if (data) {
        this.tabs.push({ component: 'config', icon: 'settings', label: 'Configuration' });
      }
    });
  }

}
