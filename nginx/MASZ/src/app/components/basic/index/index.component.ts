import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { IApplicationInfo } from 'src/app/models/IApplicationInfo';
import { ApplicationInfoService } from 'src/app/services/application-info.service';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.css']
})
export class IndexComponent implements OnInit {

  public attemptingLogin: boolean = false;
  applicationInfo?: IApplicationInfo = undefined;

  constructor(private route: ActivatedRoute, private authService: AuthService, private router: Router, private applicationInfoService: ApplicationInfoService) { }

  ngOnInit(): void {
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
    this.applicationInfoService.currentApplicationInfo.subscribe((data) => {
      this.applicationInfo = data;
    });
  }

  redirectToApiLogin() {
    this.attemptingLogin = true;
    if ('ReturnUrl' in this.route.snapshot.queryParams) {
      window.location.href=`/api/v1/login?ReturnUrl=${this.route.snapshot.queryParams['ReturnUrl']}`;
    } else {
      window.location.href="/api/v1/login";
    }
  }

}
