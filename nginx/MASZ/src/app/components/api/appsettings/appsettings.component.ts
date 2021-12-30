import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { IAppSettings } from 'src/app/models/IAppSettings';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-appsettings',
  templateUrl: './appsettings.component.html',
  styleUrls: ['./appsettings.component.css']
})
export class AppsettingsComponent implements OnInit {

  initRows: number = 2;
  settingsLoading: boolean = true;
  formGroup!: FormGroup;

  maxLength256 = { length: 256 };
  maxLength4096 = { length: 4096 };

  constructor(private toastr: ToastrService, private api: ApiService, private _formBuilder: FormBuilder, private translator: TranslateService) { }

  ngOnInit(): void {
    this.formGroup = this._formBuilder.group({
      title: ['', [ Validators.required, Validators.maxLength(256) ]],
      content: ['', [ Validators.required, Validators.maxLength(4096) ]],
    });

    this.api.getSimpleData('/settings').subscribe((data: IAppSettings) => {
      this.settingsLoading = false;
      this.initRows = Math.max(Math.min(data?.embedContent?.split(/\r\n|\r|\n/)?.length ?? 0, 15), 2);
      this.formGroup.setValue({
        title: data.embedTitle,
        content: data.embedContent
      });
    }, error => {
      this.settingsLoading = false;
      console.error(error);
    });
  }

  updateSettings() {
    let body: IAppSettings = {
      embedTitle: this.formGroup.value.title,
      embedContent: this.formGroup.value.content
    }
    this.api.putSimpleData('/settings', body, undefined, true, true).subscribe((data: IAppSettings) => {
      this.toastr.success(this.translator.instant("AppSettings.Embed.Save.Message"));
      this.settingsLoading = false;
      this.initRows = Math.max(Math.min(data?.embedContent?.split(/\r\n|\r|\n/)?.length ?? 0, 15), 2);
      this.formGroup.setValue({
        title: data.embedTitle,
        content: data.embedContent
      });
    }, error => {
      this.settingsLoading = false;
      console.error(error);
    });
  }

  get title() { return this.formGroup.get('title'); }
  get content() { return this.formGroup.get('content'); }

}
