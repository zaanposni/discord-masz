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
import { TranslateService } from '@ngx-translate/core';
import { DEFAULT_LANGUAGE, DEFAULT_TIMEZONE, LANGUAGES, TIMEZONES } from './config/config';
import { TimezoneService } from './services/timezone.service';
import { CookieTrackerService } from './services/cookie-tracker.service';
import { AppSettings } from './models/AppSettings';


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
  languages = LANGUAGES;
  timezones = TIMEZONES;
  currentAppSettings: AppSettings = {
    language: DEFAULT_LANGUAGE,
    timezone: DEFAULT_TIMEZONE
  };

  public changeLanguage(lang: string) {
    this.translator.use(lang);
    this.currentAppSettings.language = lang;
    this.cookieTracker.updateSettings(this.currentAppSettings);
  }

  public changeTimezone(zone: string) {
    this.timezoneService.timezoneChanged(zone);
    this.currentAppSettings.timezone = zone;
    this.cookieTracker.updateSettings(this.currentAppSettings);
  }

  private _mobileQueryListener: () => void;

  constructor(changeDetectorRef: ChangeDetectorRef, media: MediaMatcher, private router: Router, public route: ActivatedRoute,
              private auth: AuthService, private toastr: ToastrService, private dialog: MatDialog, private api: ApiService, private matIconRegistry: MatIconRegistry, private domSanitizer: DomSanitizer, private applicationInfoService: ApplicationInfoService, private translator: TranslateService, private timezoneService: TimezoneService, private cookieTracker: CookieTrackerService) {
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

    this.timezoneService.selectedTimezone.subscribe(data => {
      this.currentAppSettings.timezone = data;
    });

    this.api.getSimpleData('/meta/application').subscribe((data: IApplicationInfo) => {
      this.applicationInfoService.infoChanged(data);
    });

    this.translator.onLangChange.subscribe(() => {
      this.currentAppSettings.language = this.translator.currentLang;
    });

    this.cookieTracker.settings.subscribe((data: AppSettings) => {
      if (TIMEZONES.includes(data.timezone)) {  // user might enter random stuff
        this.currentAppSettings.timezone = data.timezone;
        this.timezoneService.timezoneChanged(data.timezone);
      }
      if (LANGUAGES.filter(x => x.language === data.language).length > 0) {  // user might enter random stuff
        this.currentAppSettings.language = data.language;
        this.translator.use(data.language);
      }
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
