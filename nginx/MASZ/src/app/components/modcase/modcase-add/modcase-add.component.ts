import {COMMA, ENTER, SPACE} from '@angular/cdk/keycodes';
import { HttpParams } from '@angular/common/http';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatChipInputEvent } from '@angular/material/chips';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable, ReplaySubject } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { CaseTemplate } from 'src/app/models/CaseTemplate';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { DiscordUser } from 'src/app/models/DiscordUser';
import { TemplateView } from 'src/app/models/TemplateView';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';
import { TemplateSettings, TemplateViewPermission } from 'src/app/models/TemplateSettings';
import { TemplateCreateDialogComponent } from '../../dialogs/template-create-dialog/template-create-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { AppUser } from 'src/app/models/AppUser';
import { PunishmentType } from 'src/app/models/PunishmentType';
import { APIEnum } from 'src/app/models/APIEnum';
import { EnumManagerService } from 'src/app/services/enum-manager.service';
import { APIEnumTypes } from 'src/app/models/APIEmumTypes';
import * as moment from 'moment';
import { TranslateService } from '@ngx-translate/core';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { ICaseLabel } from 'src/app/models/ICaseLabel';
import { go, highlight } from 'fuzzysort';


@Component({
  selector: 'app-modcase-add',
  templateUrl: './modcase-add.component.html',
  styleUrls: ['./modcase-add.component.css']
})
export class ModcaseAddComponent implements OnInit {

  public punishedUntilChangeForPicker: ReplaySubject<Date> = new ReplaySubject<Date>(1);
  public punishedUntil?: moment.Moment;

  public templateFormGroup!: FormGroup;
  public memberFormGroup!: FormGroup;
  public infoFormGroup!: FormGroup;
  public punishmentFormGroup!: FormGroup;
  public filesFormGroup!: FormGroup;
  public optionsFormGroup!: FormGroup;

  @ViewChild("fileInput", {static: false}) fileInput!: ElementRef;
  public filesToUpload: any[] = [];

  readonly separatorKeysCodes: number[] = [ENTER, COMMA, SPACE];
  public caseLabels: string[] = [];
  public labelInputForm: FormControl = new FormControl();
  @ViewChild('labelInput') labelInput?: ElementRef<HTMLInputElement>;
  public filteredLabels: ReplaySubject<ICaseLabel[]> = new ReplaySubject(1);
  public remoteLabels: ICaseLabel[] = [];

  public memberForm = new FormControl();
  public filteredMembers!: Observable<DiscordUser[]>;

  public templateSearch: string = "";

  public savingCase: boolean = false;

  public guildId!: string;
  public members: ContentLoading<DiscordUser[]> = { loading: true, content: [] };
  public templates: ContentLoading<TemplateView[]> = { loading: true, content: [] };
  public allTemplates: TemplateView[] = [];
  public punishmentOptions: ContentLoading<APIEnum[]> = { loading: true, content: [] };
  public currentUser!: AppUser;
  constructor(private _formBuilder: FormBuilder, private api: ApiService, private toastr: ToastrService, private authService: AuthService, private router: Router, private route: ActivatedRoute, private dialog: MatDialog, private enumManager: EnumManagerService, private translator: TranslateService) {
    this.labelInputForm.valueChanges.subscribe(data => {
      const localeLowerCaseCopy = this.caseLabels.slice().map(x => x.toLowerCase());
      this.filteredLabels.next(data ? this._filterLabel(data) : this.remoteLabels.slice(0, 5).filter(x => !localeLowerCaseCopy.includes(x.label.toLowerCase())));
    });
   }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid') as string;

    this.memberFormGroup = this._formBuilder.group({
      member: ['', Validators.required]
    });
    this.infoFormGroup = this._formBuilder.group({
      title: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', Validators.required]
    });
    this.punishmentFormGroup = this._formBuilder.group({
      punishmentType: ['', Validators.required],
      dmNotification: [false],
      handlePunishment: [false]
    });
    this.filesFormGroup = this._formBuilder.group({
      files: ['']
    });
    this.optionsFormGroup = this._formBuilder.group({
      sendNotification: [false]
    });

    this.optionsFormGroup.controls['sendNotification'].setValue(true);

    this.punishmentFormGroup.get('punishmentType')?.valueChanges.subscribe((val: PunishmentType) => {
      if (val !== PunishmentType.Ban && val !== PunishmentType.Mute) {
        this.punishedUntil = undefined;
      }
      if (val === PunishmentType.None) {
        this.punishmentFormGroup.controls['handlePunishment'].setValue(false);
      } else {
        this.punishmentFormGroup.controls['handlePunishment'].setValue(true);
        this.punishmentFormGroup.controls['dmNotification'].setValue(true);
      }
    });

    this.filteredMembers = this.memberFormGroup.valueChanges
      .pipe(
        startWith(''),
        map(value => this._filter(value.member))
      );
    this.reload();
  }

  private _filter(value: string): DiscordUser[] {
    if (!value?.trim()) {
      return this.members.content?.filter(option => !option.bot)?.slice(0, 10) as DiscordUser[];
    }
    const filterValue = value.trim().toLowerCase();

    return this.members.content?.filter(option =>
       ((option.username + "#" + option.discriminator).toLowerCase().includes(filterValue) ||
       option.id.includes(filterValue)) && !option.bot).slice(0, 10) as DiscordUser[];
  }

  private _filterLabel(value: string): ICaseLabel[] {
    const localeLowerCaseCopy = this.caseLabels.slice().map(x => x.toLowerCase());
    if (! value)
    {
      return this.remoteLabels.slice(0, 5).filter(x => !localeLowerCaseCopy.includes(x.label.toLowerCase()));
    }

    const filterValue = value.trim().toLowerCase();
    return go(filterValue, this.remoteLabels.slice().filter(x => !localeLowerCaseCopy.includes(x.label.toLowerCase())), { key: 'label' }).map(r => ({
      label: highlight(r),
      cleanValue: r.obj.label,
      count: r.obj.count
    }as ICaseLabel)).sort((a, b) => b.count - a.count).slice(0, 5);
  }

  uploadInit() {
    const fileInput = this.fileInput.nativeElement;
    fileInput .onchange = () => {
      for (let index = 0; index < fileInput .files.length; index++)
      {
        const file = fileInput.files[index];
        this.filesToUpload.push({ data: file, inProgress: false, progress: 0});
      }
    };
    fileInput.click();
  }

  searchTemplate() {
    if (!this.templateSearch?.trim()) {
      this.templates = { loading: false, content: this.allTemplates.slice(0, 3) };
    }
    const search = this.templateSearch.toLowerCase();
    this.templates = { loading: false, content: this.allTemplates.filter(x =>
        x.caseTemplate.templateName.includes(search) ||
        x.caseTemplate.caseTitle.includes(search) ||
        x.caseTemplate.caseDescription.includes(search) ||
        x.caseTemplate.caseLabels.includes(search) ||

        (x.creator?.username + "#" + x.creator?.discriminator).includes(search) ||

        x.guild.name.includes(search) ||
        x.guild.id.includes(search)
      ).slice(0, 3)};
  }

  reload() {
    this.members = { loading: true, content: [] };
    this.templates = { loading: true, content: [] };
    this.punishmentOptions = { loading: true, content: [] };
    this.templateSearch = "";
    this.allTemplates = [];

    const params = new HttpParams()
          .set('partial', 'true');
    this.api.getSimpleData(`/discord/guilds/${this.guildId}/members`, true, params).subscribe(data => {
      this.members.content = data;
      this.members.loading = false;
    }, error => {
      console.error(error);
      this.members.loading = false;
      this.toastr.error(this.translator.instant('ModCaseDialog.FailedToLoad.MemberList'));
    });

    this.enumManager.getEnum(APIEnumTypes.PUNISHMENT).subscribe(data => {
      this.punishmentOptions.content = data;
      this.punishmentOptions.loading = false;
    }, error => {
      console.error(error);
      this.punishmentOptions.loading = false;
      this.toastr.error(this.translator.instant('ModCaseDialog.FailedToLoad.PunishmentEnum'));
    });

    this.reloadTemplates();

    this.authService.getUserProfile().subscribe((data) => {
      this.currentUser = data;
    });

    this.api.getSimpleData(`/guilds/${this.guildId}/cases/labels`).subscribe(labels => {
      this.remoteLabels = labels;
      this.filteredLabels.next(this._filterLabel(this.labelInputForm.value));
    });
  }

  reloadTemplates() {
    const params = new HttpParams()
          .set('guildid', this.guildId)
    this.api.getSimpleData(`/templatesview`, true, params).subscribe(data => {
      this.allTemplates = data;
      this.searchTemplate();
    }, error => {
      console.error(error);
      this.toastr.error(this.translator.instant('ModCaseDialog.FailedToLoad.CaseTemplates'));
      this.searchTemplate();
    });
  }

  applyTemplate(template: CaseTemplate, stepper: any) {
    stepper?.next();
    this.infoFormGroup.setValue({
      title: template.caseTitle,
      description: template.caseDescription
    });
    this.caseLabels = template.caseLabels;
    this.punishmentFormGroup.setValue({
      punishmentType: template.casePunishmentType,
      dmNotification: template.announceDm,
      handlePunishment: template.handlePunishment
    });
    this.optionsFormGroup.setValue({
      sendNotification: template.sendPublicNotification
    });
    if (template.casePunishedUntil) {
      this.punishedUntilChangeForPicker.next(template.casePunishedUntil);
    }
    this.toastr.success(this.translator.instant('ModCaseDialog.AppliedTemplate'));
  }

  punishedUntilChanged(date: moment.Moment) {
    this.punishedUntil = date;
  }

  createCase() {
    this.savingCase = true;
    const data = {
      title: this.infoFormGroup.value.title,
      description: this.infoFormGroup.value.description,
      userid: this.memberFormGroup.value.member?.trim(),
      labels: this.caseLabels,
      punishmentType: this.punishmentFormGroup.value.punishmentType,
      punishedUntil: this.punishedUntil?.toISOString(),
    }

    const params = new HttpParams()
      .set('sendPublicNotification', this.optionsFormGroup.value.sendNotification ? 'true' : 'false')
      .set('handlePunishment', this.punishmentFormGroup.value.handlePunishment ? 'true' : 'false')
      .set('sendDmNotification', this.punishmentFormGroup.value.dmNotification ? 'true' : 'false');

    this.api.postSimpleData(`/guilds/${this.guildId}/cases`, data, params, true, true).subscribe(data => {
      const caseId = data.caseId;
      this.router.navigate(['guilds', this.guildId, 'cases', caseId], { queryParams: { 'reloadfiles': this.filesToUpload.length ?? '0' } });
      this.savingCase = false;
      this.toastr.success(this.translator.instant('ModCaseDialog.CaseCreated'));
      this.filesToUpload.forEach(element => this.uploadFile(element.data, caseId));
    }, error => {
      console.error(error);
      this.savingCase = false;
    });
  }

  deleteTemplate(templateId: number) {
    this.api.deleteData(`/templates/${templateId}`).subscribe(() => {
      this.reloadTemplates();
      this.toastr.success(this.translator.instant('ModCaseDialog.TemplateDeleted'));
    }, error => {
      console.error(error);
      this.toastr.error(this.translator.instant('ModCaseDialog.FailedToDeleteTemplate'));
    })
  }

  saveTemplate() {
    this.savingCase = true;
    let templateSetting: TemplateSettings = {
      name: '',
      viewPermission: TemplateViewPermission.Self
    };
    const confirmDialogRef = this.dialog.open(TemplateCreateDialogComponent, {
      data: templateSetting
    });
    confirmDialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        const data = {
          templatename: templateSetting.name,
          viewPermission: templateSetting.viewPermission,
          title: this.infoFormGroup.value.title,
          description: this.infoFormGroup.value.description,
          labels: this.caseLabels,
          punishmentType: this.punishmentFormGroup.value.punishmentType,
          punishedUntil: this.punishedUntil?.toISOString(),
          sendPublicNotification: this.optionsFormGroup.value.sendNotification ?? false,
          handlePunishment: this.punishmentFormGroup.value.handlePunishment ?? false,
          announceDm: this.punishmentFormGroup.value.dmNotification ?? false
        };

        const params = new HttpParams()
          .set('guildid', this.guildId);

        this.api.postSimpleData(`/templates`, data, params, true, true).subscribe(() => {
          this.toastr.success(this.translator.instant('ModCaseDialog.TemplateSaved'));
          this.savingCase = false;
        }, error => {
          console.error(error);
          this.toastr.error(this.translator.instant('ModCaseDialog.FailedToSaveTemplate'));
          this.savingCase = false;
        });
      } else {
        this.savingCase = false;
      }
    });
  }

  uploadFile(file: any, caseId: string) {
    this.api.postFile(`/guilds/${this.guildId}/cases/${caseId}/files`, file).subscribe(() => {
      this.toastr.success(this.translator.instant('ModCaseDialog.FileUploaded'));
    }, (error) => {
      console.error(error);
      this.toastr.error(this.translator.instant('ModCaseDialog.FailedToUploadFile'));
    });
  }

  add(event: MatChipInputEvent): void {
    const input = event.input;
    const value = event.value;

    if ((value || '').trim()) {
      const localeLowerCaseCopy = this.caseLabels.slice().map(x => x.toLowerCase());
      if (! localeLowerCaseCopy.includes(value.trim().toLowerCase())) {
        this.caseLabels.push(value.trim());
      }
    }

    if (input) {
      input.value = '';
    }
  }

  remove(label: string): void {
    const index = this.caseLabels.indexOf(label);

    if (index >= 0) {
      this.caseLabels.splice(index, 1);
    }
  }

  autoCompleteSelected(event: MatAutocompleteSelectedEvent) {
    this.labelInput!.nativeElement.value = '';
    this.labelInputForm.setValue(null);
    const localeLowerCaseCopy = this.caseLabels.slice().map(x => x.toLowerCase());
    if (! localeLowerCaseCopy.includes(event.option.value.toLowerCase())) {
      this.caseLabels.push(event.option.value);
    }
  }
}

