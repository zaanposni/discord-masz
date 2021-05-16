import { ENTER, COMMA, SPACE } from '@angular/cdk/keycodes';
import { HttpParams } from '@angular/common/http';
import { Component, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatChipInputEvent } from '@angular/material/chips';
import { MatDialog } from '@angular/material/dialog';
import { MatStepper } from '@angular/material/stepper';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { startWith, map } from 'rxjs/operators';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { DiscordUser } from 'src/app/models/DiscordUser';
import { convertToDisplayPunishmentType, convertToPunishment, convertToPunishmentType, DisplayPunishmentType, DisplayPunishmentTypeOptions, ModCase } from 'src/app/models/ModCase';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-modcase-edit',
  templateUrl: './modcase-edit.component.html',
  styleUrls: ['./modcase-edit.component.css']
})
export class ModcaseEditComponent implements OnInit {

  public memberFormGroup!: FormGroup;
  public infoFormGroup!: FormGroup;
  public punishmentFormGroup!: FormGroup;
  public filesFormGroup!: FormGroup;
  public optionsFormGroup!: FormGroup;

  readonly separatorKeysCodes: number[] = [ENTER, COMMA, SPACE];
  public caseLabels: string[] = [];

  public savingCase: boolean = false;
  
  public memberForm = new FormControl();
  public filteredMembers!: Observable<DiscordUser[]>;
  
  public guildId!: string;
  public caseId!: string;
  public members: ContentLoading<DiscordUser[]> = { loading: true, content: [] };
  public oldCase: ContentLoading<ModCase> = { loading: true, content: undefined };
  public displayPunishmentTypeOptions = DisplayPunishmentTypeOptions;
  
  constructor(private _formBuilder: FormBuilder, private api: ApiService, private toastr: ToastrService, private authService: AuthService, private router: Router, private route: ActivatedRoute, private dialog: MatDialog) { }

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
      handlePunishment: [''],
      punishedUntil: ['']
    });
    this.optionsFormGroup = this._formBuilder.group({
      sendNotification: ['']
    });

    this.optionsFormGroup.controls['sendNotification'].setValue(true);

    this.punishmentFormGroup.get('punishmentType')?.valueChanges.subscribe((val: DisplayPunishmentType) => {
      if (val === DisplayPunishmentType.TempBan || val === DisplayPunishmentType.TempMute) {
        this.punishmentFormGroup.controls['punishedUntil'].setValidators(Validators.required);
        this.punishmentFormGroup.controls['punishedUntil'].updateValueAndValidity();
      } else {
        this.punishmentFormGroup.controls['punishedUntil'].clearValidators();
        this.punishmentFormGroup.controls['punishedUntil'].setValue(null);
        this.punishmentFormGroup.controls['punishedUntil'].updateValueAndValidity();
      }
      if (val > 2) {
        this.punishmentFormGroup.controls['handlePunishment'].setValue(true);
      } else {
        this.punishmentFormGroup.controls['dmNotification'].setValue(false);
        this.punishmentFormGroup.controls['handlePunishment'].setValue(false);
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

    this.api.getSimpleData(`/modcases/${this.guildId}/${this.caseId}`).subscribe((data) => {
      this.oldCase.content = data;
      this.applyOldCase(data);
      this.oldCase.loading = false;
    }, () => {
      this.toastr.error("Failed to load current case.");
      this.oldCase.loading = false;
    });

    let params = new HttpParams()
          .set('partial', 'true');
    this.api.getSimpleData(`/discord/guilds/${this.guildId}/members`, true, params).subscribe((data) => {
      this.members.content = data;      
      this.members.loading = false;
    }, () => {
      this.toastr.error("Failed to load member list.");
    });
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
      punishmentType: convertToDisplayPunishmentType(modCase.punishmentType, modCase.punishment, modCase.punishedUntil),
      dmNotification: false,
      handlePunishment: false,
      punishedUntil: modCase.punishedUntil
    });
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
      punishment: convertToPunishment(this.punishmentFormGroup.value.punishmentType),
      punishmentType: convertToPunishmentType(this.punishmentFormGroup.value.punishmentType),
      punishedUntil: (typeof this.punishmentFormGroup.value.punishedUntil === 'string') ? this.punishmentFormGroup.value.punishedUntil : this.punishmentFormGroup.value.punishedUntil?.toISOString() ?? null,
    };
    const params = new HttpParams()
      .set('sendnotification', this.optionsFormGroup.value.sendNotification ? 'true' : 'false')
      .set('handlePunishment', this.punishmentFormGroup.value.handlePunishment ? 'true' : 'false')
      .set('announceDm', this.punishmentFormGroup.value.dmNotification ? 'true' : 'false');

      this.api.putSimpleData(`/modcases/${this.guildId}/${this.caseId}`, data, params, true, true).subscribe((data) => {     
        const caseId = data.caseid;
        this.router.navigate(['guilds', this.guildId, 'cases', caseId]);
        this.savingCase = false;
        this.toastr.success(`Case ${caseId} updated.`);
      }, () => { this.savingCase = false; });
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
