import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { InfoPanel } from '../models/InfoPanel';
import { ApiService } from '../services/api.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.scss']
})
export class IndexComponent implements OnInit {

  infoPanels!: Observable<InfoPanel[]>;
  attemptingLogin: boolean = false;

  constructor(private authService: AuthService, private api: ApiService, private router: Router) { }

  ngOnInit(): void {
    this.infoPanels = this.api.getSimpleData('/static/indexpage.json?v=1.9', false);
    if (this.authService.isLoggedIn()) {
      this.attemptingLogin = true;
      this.authService.getUserProfile().subscribe((success) => {
        this.attemptingLogin = false;
        this.router.navigate(['guilds']);
      }, () => {
        this.attemptingLogin = false;
      });
    }
  }

  isLoggedIn() {
    
    return true;
  }

}
