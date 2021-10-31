import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { forkJoin, interval, ReplaySubject } from 'rxjs';
import { Adminstats } from 'src/app/models/Adminstats';
import { AppVersion } from 'src/app/models/AppVersion';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { ApiService } from 'src/app/services/api.service';
import { ConfirmationDialogComponent } from '../../dialogs/confirmation-dialog/confirmation-dialog.component';
import { compare } from 'compare-versions';
import { IImageVersion } from 'src/app/models/IImageVersion';

@Component({
  selector: 'app-adminstats',
  templateUrl: './adminstats.component.html',
  styleUrls: ['./adminstats.component.css']
})
export class AdminstatsComponent implements OnInit {

  private subscription?: any;
  private timeDifference?: number;
  public secondsToNewCache?: string = '--';
  public minutesToNewCache?: string = '--';
  public stats: ContentLoading<Adminstats> = { loading: true, content: undefined };
  public localVersion: ContentLoading<AppVersion> = { loading: true, content: undefined };
  public availableVersions: ContentLoading<IImageVersion[]> = { loading: true, content: [] };
  public newVersionFound: ReplaySubject<IImageVersion> = new ReplaySubject(1);

  constructor(private api: ApiService, private toastr: ToastrService, private dialog: MatDialog, private translator: TranslateService) { }

  ngOnInit(): void {
    this.reload();
  }

  public reload() {
    this.stats = { loading: true, content: undefined };
    this.localVersion = { loading: true, content: undefined };
    this.availableVersions = { loading: true, content: [] };
    this.newVersionFound.next(undefined);
    this.subscription?.unsubscribe();

    this.api.getSimpleData(`/meta/adminstats`).subscribe((data: Adminstats) => {
      this.stats = { loading: false, content: data };
      this.subscription = interval(1000)
           .subscribe(x => { this.getTimeDifference(); });
    }, error => {
      console.error(error);
      this.stats.loading = false;
      this.toastr.error(this.translator.instant('Adminstats.FailedToLoad'));
    });

    let localVersionObservable = this.api.getSimpleData(`/static/version.json`, false)
    localVersionObservable.subscribe((data: AppVersion) => {
      this.localVersion.loading = false;
      this.localVersion.content = {
        version: data.version.replace('v', ''),
        pre_release: data.pre_release,
      }
    }, error => {
      console.error(error);
      this.localVersion.loading = false;
      this.toastr.error(this.translator.instant('Adminstats.FailedToLoadLocalVersion'));
    });

    let availableVersionsObservable = this.api.getSimpleData(`/meta/versions`)
    availableVersionsObservable.subscribe((data: IImageVersion[]) => {
      this.availableVersions = { loading: false, content: data };
    }, error => {
      console.error(error);
      this.availableVersions.loading = false;
      this.toastr.error(this.translator.instant('Adminstats.FailedToLoadAvailableVersions'));
    });

    forkJoin([localVersionObservable, availableVersionsObservable]).subscribe(() => {
      setTimeout(() => {
        let newestVersion = this.availableVersions.content?.find(x => x !== undefined);
        let localVersionTag = this.localVersion.content?.version;
        if (newestVersion != undefined && localVersionTag != undefined) {
          if (compare(newestVersion.tag, localVersionTag, '>')) {
            this.newVersionFound.next(newestVersion);
          }
        }
      }, 100);
    });
  }

  private getTimeDifference() {
    if (this.stats.content == undefined) return;
    if (new Date(this.stats.content!.nextCache ?? '') < new Date()) {
      this.reload();
      return;
    }
    this.timeDifference = new Date(this.stats.content!.nextCache ?? '').getTime() - new Date().getTime();
    let seconds = Math.floor((this.timeDifference) / 1000 % 60);
    this.secondsToNewCache = seconds < 10 ? `0${seconds}` : seconds.toString();
    let minutes = Math.floor((this.timeDifference) / (1000 * 60) % 60);
    this.minutesToNewCache = minutes < 10 ? `0${minutes}` : minutes.toString();
  }

  ngOnDestroy() {
    this.subscription?.unsubscribe();
  }

  public triggerCache() {
    const confirmDialogRef = this.dialog.open(ConfirmationDialogComponent);
    confirmDialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.api.postSimpleData(`/meta/cache`, {}).subscribe(() => {
          this.stats = { loading: true, content: undefined };
          this.toastr.success(this.translator.instant('Adminstats.Cleared'));
          setTimeout(() => {
            this.reload();
          }, 2000);
        }, (error) => {
          console.error(error);
          this.toastr.error(this.translator.instant('Adminstats.FailedToClear'));
        });
      }
    });
  }
}
