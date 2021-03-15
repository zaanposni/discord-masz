import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
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

  constructor(private authService: AuthService, private api: ApiService, private router: Router, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.infoPanels = this.api.getSimpleData('/static/indexpage.json?v=1.10', false);
    if (this.authService.isLoggedIn()) {
      this.attemptingLogin = true;
      this.authService.getUserProfile().subscribe((success) => {
        this.attemptingLogin = false;
        if ('ReturnUrl' in this.route.snapshot.queryParams) {
          this.router.navigateByUrl(this.route.snapshot.queryParams['ReturnUrl']);
        } else {
          this.router.navigate(['guilds']);
        }
      }, () => {
        this.attemptingLogin = false;
      });
    }
  }

  redirectToApiLogin() {
    if ('ReturnUrl' in this.route.snapshot.queryParams) {
      window.location.href=`/api/v1/login?ReturnUrl=${this.route.snapshot.queryParams['ReturnUrl']}`;
    } else {
      window.location.href="/api/v1/login";
    }
  }

}
