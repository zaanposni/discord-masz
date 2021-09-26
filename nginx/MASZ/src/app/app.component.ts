import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { MediaMatcher } from '@angular/cdk/layout';
import { ActivatedRoute, ActivationEnd, NavigationEnd, Router, UrlSegment } from '@angular/router';
import { AuthService } from './services/auth.service';
import { AppUser } from './models/AppUser';
import { ToastrService } from 'ngx-toastr';
import { GuildDeleteDialogComponent } from './components/guilds/guild-delete-dialog/guild-delete-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { Guild } from './models/Guild';
import { GuildDeleteDialogData } from './models/GuildDeleteDialogData';
import { ConfirmationDialogComponent } from './components/dialogs/confirmation-dialog/confirmation-dialog.component';
import { HttpParams } from '@angular/common/http';
import { ApiService } from './services/api.service';
import { MatIconRegistry } from '@angular/material/icon';
import { DomSanitizer } from '@angular/platform-browser';
import { IApplicationInfo } from './models/IApplicationInfo';
import { ApplicationInfoService } from './services/application-info.service';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'MASZ';
  mobileQuery: MediaQueryList;
  activatedNav: string[] = ['', ''];
  loggedIn: boolean = false;
  currentUser!: AppUser;
  guildDeleteDialogData!: GuildDeleteDialogData;
  @ViewChild('snav') snav: any;
  applicationInfo?: IApplicationInfo = undefined;

  private _mobileQueryListener: () => void;

  constructor(changeDetectorRef: ChangeDetectorRef, media: MediaMatcher, private router: Router, public route: ActivatedRoute,
              private auth: AuthService, private toastr: ToastrService, private dialog: MatDialog, private api: ApiService, private matIconRegistry: MatIconRegistry, private domSanitizer: DomSanitizer, private applicationInfoService: ApplicationInfoService) {
    this.mobileQuery = media.matchMedia('(max-width: 1000px)');
    this._mobileQueryListener = () => changeDetectorRef.detectChanges();
    this.mobileQuery.addListener(this._mobileQueryListener);

    this.matIconRegistry.addSvgIcon(
      "githublogo",
      this.domSanitizer.bypassSecurityTrustResourceUrl("/assets/img/github.svg"));
  }

  ngOnInit(): void {
    this.router.events.subscribe((data) => {
      if (data instanceof NavigationEnd) {
        this.activatedNav = data.url.split('?')[0].split('/');
      }
    });

    if (this.auth.isLoggedIn()) {
      this.login();
    }

    this.applicationInfoService.currentApplicationInfo.subscribe(data => {
      this.applicationInfo = data;
    });

    this.api.getSimpleData('/meta/application').subscribe((data: IApplicationInfo) => {
      this.applicationInfoService.infoChanged(data);
    });
  }

  login() {
    this.loggedIn = true;
      this.auth.getUserProfile().subscribe((data) => {
        this.loggedIn = true;
        this.currentUser = data;
      }, () => {
        this.loggedIn = false;
        this.currentUser = { } as AppUser;
    });
  }

  ngAfterViewInit(): void {    
    if (! this.mobileQuery.matches) {
      this.snav?.open();
    }
  }

  ngOnDestroy(): void {
    this.mobileQuery.removeListener(this._mobileQueryListener);
  }

  deleteGuild(guild: Guild) {
    this.guildDeleteDialogData = {
      guild: guild,
      deleteData: false
    }
    const dialogRef = this.dialog.open(GuildDeleteDialogComponent, {
      data: this.guildDeleteDialogData
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const confirmDialogRef = this.dialog.open(ConfirmationDialogComponent);
        confirmDialogRef.afterClosed().subscribe(confirmed => {
          if (confirmed) {
            let params = new HttpParams()
              .set('deletedata', this.guildDeleteDialogData.deleteData ? 'true' : 'false');
            this.api.deleteData(`/guilds/${this.guildDeleteDialogData.guild.id}`, params).subscribe((data) => {
              this.toastr.success('Guild deleted.');
              this.auth.resetCache();
              this.login();
            }, (error) => {
              this.toastr.error('Cannot delete guild.', 'Something went wrong.');
            });
          }
        });
      }
    });
  }

  open(...target: any[]) {
    const url = target.join('/');
    if (url === 'guilds' && !this.loggedIn) {
      this.toastr.warning('Please login first.')
    } else {
      this.router.navigateByUrl(url);
    }

    if (this.mobileQuery.matches) {
      this.snav?.toggle();
    }
  }
}
