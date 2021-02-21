import { HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Meta } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AppUser } from 'src/app/models/AppUser';
import { GitHubRelease } from 'src/app/models/GitHubRelease';
import { MetaVersion } from 'src/app/models/MetaVersion';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {
  currentUser!: Observable<AppUser>;
  userId: string;
  currentVersion!: MetaVersion;
  githubReleases: GitHubRelease[] = [];

  constructor(private authService: AuthService, public router: Router, private api: ApiService) { }

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.currentUser = this.authService.getUserProfile();
      this.currentUser.subscribe((data) => {
        this.userId = data?.discordUser?.id;
      });
    }
    this.api.getSimpleData("/static/version.json", false, new HttpParams(), false).subscribe((data) => {
      this.currentVersion = data;
    });
    this.api.getSimpleData("/meta/githubreleases", true, new HttpParams(), false).subscribe((data) => {
      this.githubReleases = data;
    });
  }
  	
  navigateToProfile() {
    this.router.navigate(['profile', this.userId]);
  }

  showUpdateWarning() {
    if (this.currentVersion?.pre_release) {
      Swal.fire({
        title: `Prerelease ${this.currentVersion?.version} detected!`,
        text: `The last stable release is ${this.githubReleases[0]?.tag_name}.`,
        icon: 'info',
      });
    } else {
      Swal.fire({
        title: `New version ${this.githubReleases[0]?.tag_name} found!`,
        text: `You are still on version ${this.currentVersion?.version}. It is recommended to update as soon as possible.`,
        icon: 'info',
      });
    }
  }
}
