import { ENTER, COMMA, SPACE, X } from '@angular/cdk/keycodes';
import { HttpParams } from '@angular/common/http';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatChipInputEvent } from '@angular/material/chips';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { Observable, ReplaySubject } from 'rxjs';
import { startWith, map } from 'rxjs/operators';
import { APIEnumTypes } from 'src/app/models/APIEmumTypes';
import { APIEnum } from 'src/app/models/APIEnum';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { DiscordUser } from 'src/app/models/DiscordUser';
import { ModCase } from 'src/app/models/ModCase';
import { PunishmentType } from 'src/app/models/PunishmentType';
import { ApiService } from 'src/app/services/api.service';
import { EnumManagerService } from 'src/app/services/enum-manager.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { ICaseLabel } from 'src/app/models/ICaseLabel';
import { go, highlight } from 'fuzzysort';


@Component({
  selector: 'app-modcase-edit',
  templateUrl: './modcase-edit.component.html',
  styleUrls: ['./modcase-edit.component.css']
})
export class ModcaseEditComponent implements OnInit {

  public punishedUntilChangeForPicker: ReplaySubject<Date> = new ReplaySubject<Date>(1);
  public punishedUntil?: moment.Moment;

  public memberFormGroup!: FormGroup;
  public infoFormGroup!: FormGroup;
  public punishmentFormGroup!: FormGroup;
  public filesFormGroup!: FormGroup;
  public optionsFormGroup!: FormGroup;

  readonly separatorKeysCodes: number[] = [ENTER, COMMA, SPACE];
  public caseLabels: string[] = [];
  public labelInputForm: FormControl = new FormControl();
  @ViewChild('labelInput') labelInput?: ElementRef<HTMLInputElement>;
  public filteredLabels: ReplaySubject<ICaseLabel[]> = new ReplaySubject(1);
  public remoteLabels: ICaseLabel[] = [];

  public savingCase: boolean = false;

  public memberForm = new FormControl();
  public filteredMembers!: Observable<DiscordUser[]>;

  public guildId!: string;
  public caseId!: string;
  public members: ContentLoading<DiscordUser[]> = { loading: true, content: [] };
  public oldCase: ContentLoading<ModCase> = { loading: true, content: undefined };
  public punishmentOptions: ContentLoading<APIEnum[]> = { loading: true, content: [] };

  constructor(private _formBuilder: FormBuilder, private api: ApiService, private toastr: ToastrService, private translator: TranslateService, private router: Router, private route: ActivatedRoute, private enumManager: EnumManagerService) {
    this.labelInputForm.valueChanges.subscribe(data => {
      const localeLowerCaseCopy = this.caseLabels.slice().map(x => x.toLowerCase());
      this.filteredLabels.next(data ? this._filterLabel(data) : this.remoteLabels.slice(0, 5).filter(x => !localeLowerCaseCopy.includes(x.label.toLowerCase())));
    });
  }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid') as string;
    this.caseId = this.route.snapshot.paramMap.get('caseid') as string;

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
      handlePunishment: ['']
    });
    this.optionsFormGroup = this._formBuilder.group({
      sendNotification: ['']
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
      }
    });

    this.filteredMembers = this.memberFormGroup.valueChanges
      .pipe(
        startWith(''),
        map(value => this._filter(value.member))
      );
    this.reload();
  }

  reload() {
    this.members = { loading: true, content: [] };
    this.oldCase = { loading: true, content: undefined };

    this.api.getSimpleData(`/guilds/${this.guildId}/cases/${this.caseId}`).subscribe(data => {
      this.oldCase.content = data;
      this.applyOldCase(data);
      this.oldCase.loading = false;
    }, error => {
      console.error(error);
      this.toastr.error(this.translator.instant('ModCaseDialog.FailedToLoad.CurrentCase'));
      this.oldCase.loading = false;
    });

    this.enumManager.getEnum(APIEnumTypes.PUNISHMENT).subscribe(data => {
      this.punishmentOptions.content = data;
      this.punishmentOptions.loading = false;
    }, error => {
      console.error(error);
      this.punishmentOptions.loading = false;
      this.toastr.error(this.translator.instant('ModCaseDialog.FailedToLoad.PunishmentEnum'));
    });

    let params = new HttpParams()
          .set('partial', 'true');
    this.api.getSimpleData(`/discord/guilds/${this.guildId}/members`, true, params).subscribe((data) => {
      this.members.content = data;
      this.members.loading = false;
    }, () => {
      this.toastr.error(this.translator.instant('ModCaseDialog.FailedToLoad.MemberList'));
    });

    this.api.getSimpleData(`/guilds/${this.guildId}/cases/labels`).subscribe(labels => {
      this.remoteLabels = labels;
      this.filteredLabels.next(this._filterLabel(this.labelInputForm.value));
    });
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

  private _filter(value: string): DiscordUser[] {
    if (!value?.trim()) {
      return this.members.content?.filter(option => !option.bot)?.slice(0, 10) as DiscordUser[];
    }
    const filterValue = value.trim().toLowerCase();

    return this.members.content?.filter(option =>
       ((option.username + "#" + option.discriminator).toLowerCase().includes(filterValue) ||
       option.id.includes(filterValue)) && !option.bot).slice(0, 10) as DiscordUser[];
  }

  public applyOldCase(modCase: ModCase) {
    this.memberFormGroup.setValue({
      member: modCase.userId
    });
    this.infoFormGroup.setValue({
      title: modCase.title,
      description: modCase.description
    });
    this.caseLabels = modCase.labels;
    this.punishmentFormGroup.setValue({
      punishmentType: modCase.punishmentType,
      dmNotification: false,
      handlePunishment: false
    });
    if (modCase.punishedUntil) {
      this.punishedUntilChangeForPicker.next(modCase.punishedUntil);
    }
    this.optionsFormGroup.setValue({
      sendNotification: false
    });
  }

  public updateCase() {
    this.savingCase = true;
    const data = {
      title: this.infoFormGroup.value.title,
      description: this.infoFormGroup.value.description,
      userid: this.memberFormGroup.value.member?.trim(),
      labels: this.caseLabels,
      punishmentType: this.punishmentFormGroup.value.punishmentType,
      punishedUntil: this.punishedUntil?.toISOString(),
    };
    const params = new HttpParams()
      .set('sendNotification', this.optionsFormGroup.value.sendNotification ? 'true' : 'false')
      .set('handlePunishment', this.punishmentFormGroup.value.handlePunishment ? 'true' : 'false')
      .set('sendDmNotification', this.punishmentFormGroup.value.dmNotification ? 'true' : 'false');

      this.api.putSimpleData(`/guilds/${this.guildId}/cases/${this.caseId}`, data, params, true, true).subscribe((data) => {
        const caseId = data.caseId;
        this.router.navigate(['guilds', this.guildId, 'cases', caseId]);
        this.savingCase = false;
        this.toastr.success(this.translator.instant('ModCaseDialog.CaseUpdated'));
      }, error => {
        console.error(error);
        this.savingCase = false;
      });
  }

  punishedUntilChanged(date: moment.Moment) {
    this.punishedUntil = date;
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
