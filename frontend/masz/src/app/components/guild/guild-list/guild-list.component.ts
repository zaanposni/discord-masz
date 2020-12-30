import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AppUser } from 'src/app/models/AppUser';
import { Guild } from 'src/app/models/Guild';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';
import { CommonModule } from '@angular/common';
import { Observable, ReplaySubject, Subject } from 'rxjs';

@Component({
  selector: 'app-guild-list',
  templateUrl: './guild-list.component.html',
  styleUrls: ['./guild-list.component.scss']
})
export class GuildListComponent implements OnInit {

  constructor(private router: ActivatedRoute, private api: ApiService, private auth: AuthService) { }

  currentUser!: Observable<AppUser>;

  ngOnInit(): void {
    this.currentUser = this.auth.getUserProfile().pipe((data) => {
      return data;
    });
  }
}
