import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppUser } from 'src/app/models/AppUser';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-guild-list',
  templateUrl: './guild-list.component.html',
  styleUrls: ['./guild-list.component.css']
})
export class GuildListComponent implements OnInit {

  public currentUser: ContentLoading<AppUser> = { loading: true, content: {} as AppUser }

  constructor(private auth: AuthService, private router: Router) { }

  ngOnInit(): void {
    this.login();
  }

  login() {
    this.currentUser = { loading: true, content: {} as AppUser };
    this.auth.getUserProfile().subscribe((data) => {
      this.currentUser = { loading: false, content: data };
    }, () => { this.currentUser.loading = false });
  }

  open(...target: any[]) {
    const url = target.join('/');
    this.router.navigateByUrl(url);    
  }
}
