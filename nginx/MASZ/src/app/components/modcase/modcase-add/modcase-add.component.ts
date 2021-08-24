import {COMMA, ENTER, SPACE} from '@angular/cdk/keycodes';
import { HttpParams } from '@angular/common/http';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatChipInputEvent } from '@angular/material/chips';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
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
import { PunishmentType, PunishmentTypeOptions } from 'src/app/models/PunishmentType';

@Component({
  selector: 'app-modcase-add',
  templateUrl: './modcase-add.component.html',
  styleUrls: ['./modcase-add.component.css']
})
export class ModcaseAddComponent implements OnInit {

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

  public memberForm = new FormControl();
  public filteredMembers!: Observable<DiscordUser[]>;

  public templateSearch: string = "";
  
  public savingCase: boolean = false;

  public guildId!: string;
  public members: ContentLoading<DiscordUser[]> = { loading: true, content: [] };
  public templates: ContentLoading<TemplateView[]> = { loading: true, content: [] };
  public allTemplates: TemplateView[] = [];
  public displayPunishmentTypeOptions = PunishmentTypeOptions;
  public currentUser!: AppUser;
  constructor(private _formBuilder: FormBuilder, private api: ApiService, private toastr: ToastrService, private authService: AuthService, private router: Router, private route: ActivatedRoute, private dialog: MatDialog) { }

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
      dmNotification: [''],
      handlePunishment: [''],
      punishedUntil: ['']
    });
    this.filesFormGroup = this._formBuilder.group({
      files: ['']
    });
    this.optionsFormGroup = this._formBuilder.group({
      sendNotification: ['']
    });

    this.optionsFormGroup.controls['sendNotification'].setValue(true);

    this.punishmentFormGroup.get('punishmentType')?.valueChanges.subscribe((val: PunishmentType) => {
      if (val !== PunishmentType.Ban && val !== PunishmentType.Mute) {
        this.punishmentFormGroup.controls['punishedUntil'].setValue(null);
        this.punishmentFormGroup.controls['punishedUntil'].updateValueAndValidity();
      }
      if (val === PunishmentType.None) {
        this.punishmentFormGroup.controls['handlePunishment'].setValue(false);
      } else {
        this.punishmentFormGroup.controls['handlePunishment'].setValue(true);
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

  uploadInit() {
    const fileInput = this.fileInput.nativeElement;
    fileInput .onchange = () => {
      for (let index = 0; index < fileInput .files.length; index++)
      {
        const file = fileInput .files[index];        
        
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
        x.caseTemplate.casePunishment.includes(search) ||
        x.caseTemplate.caseLabels.includes(search) ||

        (x.creator?.username + "#" + x.creator?.discriminator).includes(search) ||

        x.guild.name.includes(search) ||
        x.guild.id.includes(search)
      ).slice(0, 3)};
  }

  reload() {
    this.members = { loading: true, content: [] };
    this.templates = { loading: true, content: [] };
    this.templateSearch = "";
    this.allTemplates = [];

    const params = new HttpParams()
          .set('partial', 'true');
    this.api.getSimpleData(`/discord/guilds/${this.guildId}/members`, true, params).subscribe((data) => {
      this.members.content = data;      
      this.members.loading = false;
    }, () => {
      this.toastr.error("Failed to load member list.");
    });

    this.reloadTemplates();

    this.authService.getUserProfile().subscribe((data) => {
      this.currentUser = data;
    });
  }

  reloadTemplates() {
    const params = new HttpParams()
          .set('guildid', this.guildId)
    this.api.getSimpleData(`/templatesview`, true, params).subscribe((data) => {
      this.allTemplates = data;
      this.searchTemplate();
    }, () => {
      this.toastr.error("Failed to load case templates.");
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
      handlePunishment: template.handlePunishment,
      punishedUntil: template.casePunishedUntil
    });
    this.optionsFormGroup.setValue({
      sendNotification: template.sendPublicNotification
    });
    this.toastr.success(`Applied template ${template.templateName}.`);
  }

  createCase() {
    this.savingCase = true;
    const data = {
      title: this.infoFormGroup.value.title,
      description: this.infoFormGroup.value.description,
      userid: this.memberFormGroup.value.member?.trim(),
      labels: this.caseLabels,
      punishmentType: this.punishmentFormGroup.value.punishmentType,
      punishedUntil: (typeof this.punishmentFormGroup.value.punishedUntil === 'string') ? this.punishmentFormGroup.value.punishedUntil : this.punishmentFormGroup.value.punishedUntil?.toISOString() ?? null,
    }
    const params = new HttpParams()
      .set('sendnotification', this.optionsFormGroup.value.sendNotification ? 'true' : 'false')
      .set('handlePunishment', this.punishmentFormGroup.value.handlePunishment ? 'true' : 'false')
      .set('announceDm', this.punishmentFormGroup.value.dmNotification ? 'true' : 'false');
    
    this.api.postSimpleData(`/modcases/${this.guildId}`, data, params, true, true).subscribe((data) => {     
      const caseId = data.caseId;
      this.router.navigate(['guilds', this.guildId, 'cases', caseId], { queryParams: { 'reloadfiles': this.filesToUpload.length ?? '0' } });
      this.savingCase = false;
      this.toastr.success(`Case ${caseId} created.`);
      this.filesToUpload.forEach(element => this.uploadFile(element.data, caseId));
    }, () => { this.savingCase = false; });
  }

  deleteTemplate(templateId: number) {
    this.api.deleteData(`/templates/${templateId}`).subscribe(() => {
      this.reloadTemplates();
      this.toastr.success("Template deleted.");
    }, () => {
      this.toastr.error("Failed to delete template.");
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
          punishedUntil: (typeof this.punishmentFormGroup.value.punishedUntil === 'string') ? this.punishmentFormGroup.value.punishedUntil : this.punishmentFormGroup.value.punishedUntil?.toISOString() ?? null,
          sendPublicNotification: this.optionsFormGroup.value.sendNotification,
          handlePunishment: this.punishmentFormGroup.value.handlePunishment,
          announceDm: this.punishmentFormGroup.value.dmNotification
        };

        const params = new HttpParams()
          .set('guildid', this.guildId);

        this.api.postSimpleData(`/templates`, data, params, true, true).subscribe((data) => {
          this.toastr.success("Template saved.");
          this.savingCase = false;
        }, () => { this.savingCase = false; });
      } else {
        this.savingCase = false;
      }
    });
  }

  uploadFile(file: any, caseId: string) {
    this.api.postFile(`/guilds/${this.guildId}/modcases/${caseId}/files`, file).subscribe((data) => {
      this.toastr.success(`File ${file.data.name} uploaded.`);
    }, (error) => {
      this.toastr.error('Failed to upload file.');
    });
  }

  add(event: MatChipInputEvent): void {
    const input = event.input;
    const value = event.value;

    if ((value || '').trim()) {
      this.caseLabels.push(value.trim());
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
}

