import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { APIToken } from 'src/app/models/APIToken';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { NewTokenDialog } from 'src/app/models/NewTokenDialog';
import { ApiService } from 'src/app/services/api.service';
import { NewTokenDialogComponent } from '../../dialogs/new-token-dialog/new-token-dialog.component';

@Component({
  selector: 'app-token-overview',
  templateUrl: './token-overview.component.html',
  styleUrls: ['./token-overview.component.css']
})
export class TokenOverviewComponent implements OnInit {

  public tokens: ContentLoading<APIToken[]> = { loading: true, content: [] };

  public generatingNewToken: boolean = false;
  public newToken: string = '';

  constructor(private api: ApiService, private toastr: ToastrService, private dialog: MatDialog, private translator: TranslateService) { }

  ngOnInit(): void {
    this.reloadToken();
  }

  reloadToken() {
    this.tokens = { loading: true, content: [] };
    this.api.getSimpleData(`/token`).subscribe(data => {
      this.tokens.content?.push(data);
      this.tokens.loading = false;
    }, error => {
      this.tokens.loading = false;
      if (error?.error?.status !== 404 && error?.status !== 404) {
        console.error(error);
        this.toastr.error(this.translator.instant('TokenOverview.FailedToLoad'));
      }
    })
  }

  createToken() {
    var tokenDialogData: NewTokenDialog = {
      name: ''
    };
    const confirmDialogRef = this.dialog.open(NewTokenDialogComponent, {
      data: tokenDialogData
    });
    confirmDialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.generatingNewToken = true;
        this.api.postSimpleData(`/token`, tokenDialogData).subscribe(data => {
          this.generatingNewToken = false;
          this.newToken = data.token;
          this.toastr.success(this.translator.instant('TokenOverview.Created'))
          this.reloadToken();
        }, error => {
          console.error(error);
          this.toastr.error(this.translator.instant('TokenOverview.FailedToCreate'));
          this.generatingNewToken = false;
        });
      }
    });
  }

  copyTokenToClipboard() {
    navigator.clipboard.writeText(this.newToken).then(() => {
      this.toastr.success(this.translator.instant('TokenOverview.CopiedClipboard'));
    }).catch(e => console.error(e));
  }

  deleteToken(id: number) {
    this.api.deleteData(`/token`).subscribe(() => {
      this.reloadToken();
      this.toastr.success(this.translator.instant('TokenOverview.Deleted'));
    }, error => {
      console.error(error);
      this.toastr.error(this.translator.instant('TokenOverview.FailedToDelete'));
    });
  }
}
