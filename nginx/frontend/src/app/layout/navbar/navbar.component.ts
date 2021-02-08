import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AppUser } from 'src/app/models/AppUser';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {
  currentUser!: Observable<AppUser>;
  userId: string;

  constructor(private authService: AuthService, public router: Router) { }

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.currentUser = this.authService.getUserProfile();
      this.currentUser.subscribe((data) => {
        this.userId = data?.discordUser?.id;
      });
    }
  }
  	
  navigateToProfile() {
    this.router.navigate(['profile', this.userId]);
  }
}
