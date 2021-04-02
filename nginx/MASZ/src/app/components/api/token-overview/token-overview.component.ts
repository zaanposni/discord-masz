import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
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

  constructor(private api: ApiService, private toastr: ToastrService, private dialog: MatDialog) { }

  ngOnInit(): void {
    this.reloadToken();
  }

  reloadToken() {
    this.tokens = { loading: true, content: [] };
    this.api.getSimpleData(`/token`).subscribe((data) => {
      this.tokens.content?.push(data);
      this.tokens.loading = false;
    }, (error) => {
      this.tokens.loading = false;
      if (error?.status === 404) return; 
      this.toastr.error("Failed to load current tokens.");
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
        this.api.postSimpleData(`/token`, tokenDialogData).subscribe((data) => {
          this.generatingNewToken = false;
          this.newToken = data.generatedToken;
          this.reloadToken();
        }, () => {
          this.toastr.error("Failed to generate token.");
          this.generatingNewToken = false;
        });
      }
    });
  }

  deleteToken(id: number) {
    this.api.deleteData(`/token`).subscribe((data) => {
      this.reloadToken();
      this.toastr.success("Token deleted.");
    }, () => {
      this.toastr.error("Failed to delete token.");
    });
  }
}
