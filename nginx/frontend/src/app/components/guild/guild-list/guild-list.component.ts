import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { AppUser } from 'src/app/models/AppUser';
import { Guild } from 'src/app/models/Guild';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';
import { Observable, ReplaySubject, Subject } from 'rxjs';
import { CookieService } from 'ngx-cookie';
import Swal from 'sweetalert2'
import { HttpParams } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { CookieTrackerService } from 'src/app/services/cookie-tracker.service';

@Component({
  selector: 'app-guild-list',
  templateUrl: './guild-list.component.html',
  styleUrls: ['./guild-list.component.scss']
})
export class GuildListComponent implements OnInit {

  constructor(private api: ApiService, private auth: AuthService, public cookieTracker: CookieTrackerService, public router: Router, private toastr: ToastrService) { }

  currentUser!: AppUser;
  showSuggestions: boolean = false;
  loading: boolean = true;

  hideSuggestions() {
    this.cookieTracker.updateCookie('suggestions', 'false');
  }

  ngOnInit(): void {
    this.auth.getUserProfile().subscribe((data) => {
      this.loading = false;
      this.currentUser = data;
    }, () => { this.loading = false });
    this.cookieTracker.currentSettings.subscribe(data => this.showSuggestions = data.showSuggestions);
  }

  deleteGuild(guildId: string, guildName: string) {
    Swal.fire({
      title: 'Caution!',
      text: `Do you want to delete this guild? (Name: ${guildName}, Id: ${guildId})`,
      icon: 'warning',
      confirmButtonText: 'Delete',
      showCancelButton: true
    }).then((data) => {
      if(data.isConfirmed) {
        Swal.fire({
          title: 'Are you sure?',
          text: `Do you want to delete this guild? (Name: ${guildName}, Id: ${guildId})`,
          icon: 'warning',
          confirmButtonText: 'Delete',
          input: 'checkbox',
          inputValue: 1,
          inputPlaceholder: 'Delete all cases, files, moderationevents and configurations?',
          showCancelButton: true
        }).then((data) => {
          if(data.isConfirmed) {
            let params = new HttpParams()
              .set('deletedata', data?.value ? 'true' : 'false');
            this.api.deleteData(`/guilds/${guildId}`, params).subscribe((data) => {
              this.toastr.success('Guild deleted.');
              this.auth.getUserProfile(true).subscribe((data) => {
                this.currentUser = data;
              });
            }, (error) => {
              this.toastr.error('Cannot delete guild.', 'Something went wrong.');
            });
          }
        });
      }
    });
  }
}
