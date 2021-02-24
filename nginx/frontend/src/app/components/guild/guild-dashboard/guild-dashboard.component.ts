import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-guild-dashboard',
  templateUrl: './guild-dashboard.component.html',
  styleUrls: ['./guild-dashboard.component.scss']
})
export class GuildDashboardComponent implements OnInit {

  private guildId: string;
  public isAdmin!: Observable<boolean>;

  constructor(private route: ActivatedRoute, private auth: AuthService, private toastr: ToastrService, private router: Router, private api: ApiService) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid');

    this.auth.isModInGuild(this.guildId).subscribe((data) => {
      if (!data) {
        this.toastr.error("Unauthorized.");
        this.router.navigate(['guilds']);
      }
    });

    this.isAdmin = this.auth.isAdminInGuild(this.guildId);
  }

  redirectToCaseCreation() {
    this.router.navigate(['guilds', this.guildId, 'cases', 'new']);
  }

  redirectToCaseTable() {
    this.router.navigate(['guilds', this.guildId]);
  }

  redirectToAutoModConfig() {
    this.router.navigate(['guilds', this.guildId, 'automod']);
  }
}
