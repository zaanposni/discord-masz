import { HttpParams } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { AppUser } from 'src/app/models/AppUser';
import { Guild } from 'src/app/models/Guild';
import { GuildChannel } from 'src/app/models/GuildChannel';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';
import Swal from 'sweetalert2'

@Component({
  selector: 'app-automod-config',
  templateUrl: './automod-config.component.html',
  styleUrls: ['./automod-config.component.scss']
})
export class AutomodConfigComponent implements OnInit {

  guildId!: string | null;
  currentUser!: Observable<AppUser>;
  guild!: Promise<Guild>;
  guildChannels!: Promise<GuildChannel[]>;

  constructor(private route: ActivatedRoute, private auth: AuthService, private toastr: ToastrService, private api: ApiService, public router: Router) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid');

    this.auth.getUserProfile().subscribe((data) => {
      if (data.adminGuilds.find(x => x.id === this.guildId) === undefined && !data.isAdmin) {
        this.toastr.error("Unauthorized.");
        this.router.navigate(['guilds']);
      }
    });

    this.guild = this.api.getSimpleData(`/discord/guilds/${this.guildId}`).toPromise();
    this.guildChannels = this.api.getSimpleData(`/discord/guilds/${this.guildId}/channels`).toPromise();
  }
}
