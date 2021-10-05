import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { APIEnumTypes } from 'src/app/models/APIEmumTypes';
import { APIEnum } from 'src/app/models/APIEnum';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { TemplateSettings, TemplateViewPermissionOptions } from 'src/app/models/TemplateSettings';
import { EnumManagerService } from 'src/app/services/enum-manager.service';

@Component({
  selector: 'app-template-create-dialog',
  templateUrl: './template-create-dialog.component.html',
  styleUrls: ['./template-create-dialog.component.css']
})
export class TemplateCreateDialogComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public settings: TemplateSettings, private enumManager: EnumManagerService, private toastr: ToastrService, private translator: TranslateService) { }

  templateViewPermissionOptions: ContentLoading<APIEnum[]> = { loading: true, content: [] };

  ngOnInit(): void {
    this.templateViewPermissionOptions.content = [];
    this.enumManager.getEnum(APIEnumTypes.VIEWPERMISSION).subscribe(data => {
      this.templateViewPermissionOptions = { loading: false, content: data };
    }, error => {
      console.error(error);
      this.templateViewPermissionOptions.loading = false;
      this.toastr.error(this.translator.instant('TemplateCreateDialog.FailedToLoadPermissions'));
    })
  }

}
