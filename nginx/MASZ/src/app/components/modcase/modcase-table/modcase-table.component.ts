import { HttpParams } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Moment } from 'moment';
import { ToastrService } from 'ngx-toastr';
import { Observable, ReplaySubject } from 'rxjs';
import { APIEnumTypes } from 'src/app/models/APIEmumTypes';
import { APIEnum } from 'src/app/models/APIEnum';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { DiscordUser } from 'src/app/models/DiscordUser';
import { IModCaseFilter } from 'src/app/models/IModCaseFilter';
import { IModCaseTable } from 'src/app/models/IModCaseTable';
import { IModCaseTableEntry } from 'src/app/models/IModCaseTableEntry';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';
import { EnumManagerService } from 'src/app/services/enum-manager.service';

@Component({
  selector: 'app-modcase-table',
  templateUrl: './modcase-table.component.html',
  styleUrls: ['./modcase-table.component.css']
})
export class ModcaseTableComponent implements OnInit {

  @Input() apiUrl: string = 'modcasetable'
  currentPage: number = 0;

  showTable: IModCaseTableEntry[] = [];
  casesTable!: IModCaseTable;
  isModOrHigher!: Observable<boolean>;
  guildId!: string;
  @Input() uniqueIdentifier: string = "casetable";
  loading: boolean = true;

  @Input() search!: string;

  // filters
  public members: ReplaySubject<DiscordUser[]> = new ReplaySubject<DiscordUser[]>(1);
  public caseCreationTypes: ReplaySubject<APIEnum[]> = new ReplaySubject<APIEnum[]>(1);
  public punishmentTypes: ReplaySubject<APIEnum[]> = new ReplaySubject<APIEnum[]>(1);
  public editedStatus: ReplaySubject<APIEnum[]> = new ReplaySubject<APIEnum[]>(1);
  public commentLockedStatus: ReplaySubject<APIEnum[]> = new ReplaySubject<APIEnum[]>(1);
  public markedToDeleteStatus: ReplaySubject<APIEnum[]> = new ReplaySubject<APIEnum[]>(1);
  public punishmentActiveStatus: ReplaySubject<APIEnum[]> = new ReplaySubject<APIEnum[]>(1);

  public editStatusCtrl: FormControl = new FormControl();
  public commentLockedCtrl: FormControl = new FormControl();
  public markDeleteCtrl: FormControl = new FormControl();
  public punishmentActiveCtrl: FormControl = new FormControl();

  public userFilterPredicate = (member: DiscordUser, search: string) =>
      `${member.username.toLowerCase()}#${member.discriminator}`.indexOf(search.toLowerCase()) > -1 || member.id == search;
  public userDisplayPredicate = (member: DiscordUser) => `${member.username.toLowerCase()}#${member.discriminator}`;
  public userIdPredicate = (member: DiscordUser) => member.id;
  public userComparePredicate = (member: DiscordUser, member2: DiscordUser) => member?.id == member2?.id;

  public enumFilterPredicate = (enumType: APIEnum, search: string) =>
      `${enumType.value.toLowerCase()}`.indexOf(search.toLowerCase()) > -1 || enumType.key.toString() == search;
  public enumDisplayPredicate = (enumType: APIEnum) => enumType.value;

  public excludePermaPunishments: boolean = false;
  public useAdvancedFilters: boolean = true;

  public apiFilter: IModCaseFilter = {};

  constructor(public router: Router, private api: ApiService, private auth: AuthService, private route: ActivatedRoute, private toastr: ToastrService, private translator: TranslateService, private enumManager: EnumManagerService) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe((data) => {
      this.guildId = data.get('guildid') as string;
      this.isModOrHigher = this.auth.isModInGuild(this.guildId);
      this.reload();
    });

    this.editStatusCtrl.valueChanges.subscribe(value => {
      this.selectedEditedStatusChanged(value);
    });
    this.commentLockedCtrl.valueChanges.subscribe(value => {
      this.selectedCommentLockedStatusChanged(value);
    });
    this.markDeleteCtrl.valueChanges.subscribe(value => {
      this.selectedDeleteStatusChanged(value);
    });
    this.punishmentActiveCtrl.valueChanges.subscribe(value => {
      this.selectedPunishmentActiveStatusChanged(value);
    });


    this.enumManager.getEnum(APIEnumTypes.CASECREATIONTYPE).subscribe(data => {
      this.caseCreationTypes.next(data);
    });
    this.enumManager.getEnum(APIEnumTypes.PUNISHMENT).subscribe(data => {
      this.punishmentTypes.next(data);
    });
    this.enumManager.getEnum(APIEnumTypes.EDITSTATUS).subscribe(data => {
      this.editedStatus.next(data);
    });
    this.enumManager.getEnum(APIEnumTypes.LOCKEDCOMMENTSTATUS).subscribe(data => {
      this.commentLockedStatus.next(data);
    });
    this.enumManager.getEnum(APIEnumTypes.MARKEDTODELETESTATUS).subscribe(data => {
      this.markedToDeleteStatus.next(data);
    });
    this.enumManager.getEnum(APIEnumTypes.PUNISHMENTACTIVESTATUS).subscribe(data => {
      this.punishmentActiveStatus.next(data);
    });
  }

  reload() {
    this.loadFirstCases();
    this.loadMembers();
  }

  private loadMembers() {
    const params = new HttpParams()
          .set('partial', 'true');
    this.api.getSimpleData(`/discord/guilds/${this.guildId}/members`, true, params).subscribe(data => {
      this.members.next(data);
    }, error => {
      console.error(error);
      this.toastr.error(this.translator.instant('ModCaseDialog.FailedToLoad.MemberList'));
    });
  }

  searchChanged(event: any) {
    this.apiFilter.customTextFilter = event;
  }

  selectedMemberChanged(members: DiscordUser[]) {
    this.apiFilter.userIds = members?.map(x => x.id) ?? [];
  }

  selectedModChanged(members: DiscordUser[]) {
    this.apiFilter.moderatorIds = members?.map(x => x.id) ?? [];
  }

  selectedCreationTypeChanged(types: APIEnum[]) {
    this.apiFilter.creationTypes = types?.map(x => x.key) ?? [];
  }

  selectedPunishmentTypeChanged(types: APIEnum[]) {
    this.apiFilter.punishmentTypes = types?.map(x => x.key) ?? [];
  }

  selectedEditedStatusChanged(type: APIEnum) {
    this.apiFilter.edited = type.key === 0 ? undefined : type.key !== 1;
  };

  selectedCommentLockedStatusChanged(type: APIEnum) {
    this.apiFilter.lockedComments = type.key === 0 ? undefined : type.key !== 1;
  };

  selectedDeleteStatusChanged(type: APIEnum) {
    this.apiFilter.markedToDelete = type.key === 0 ? undefined : type.key !== 1;
  };

  selectedPunishmentActiveStatusChanged(type: APIEnum) {
    this.apiFilter.punishmentActive = type.key === 0 ? undefined : type.key !== 1;
  };

  selectedSinceChanged(date: Moment) {
    this.apiFilter.since = date?.toISOString();
  }

  selectedUntilChanged(date: Moment) {
    this.apiFilter.before = date?.toISOString();
  }

  selectedPunishmentSinceChanged(date: Moment) {
    this.apiFilter.punishedUntilMin = date?.toISOString();
  }

  selectedPunishmentUntilChanged(date: Moment) {
    this.apiFilter.punishedUntilMax = date?.toISOString();
  }

  loadFirstCases() {
    this.loading = true;
    this.currentPage = 0;
    this.api.postSimpleData(`/guilds/${this.guildId}/${this.apiUrl}`, this.apiFilter).subscribe((data: IModCaseTable) => {
      this.loading = false;
      this.casesTable = data;
      this.applyCurrentFilters();
    }, error => {
      console.error(error);
      this.toastr.error(this.translator.instant('ModCaseTable.FailedToLoad'));
    });
  }

  loadNextPage() {
    // this.loading = true;
    this.currentPage++;
    const params = new HttpParams()
          .set('startPage', this.currentPage.toString());
    this.api.postSimpleData(`/guilds/${this.guildId}/${this.apiUrl}`, {}, params).subscribe((data: IModCaseTable) => {
      // this.loading = false;
      this.casesTable.cases.push(...data.cases);
      this.casesTable.fullSize = data.fullSize;
      this.applyCurrentFilters();
    }, error => {
      console.error(error);
      this.toastr.error(this.translator.instant('ModCaseTable.FailedToLoad'));
    });
  }

  applyCurrentFilters() {
    let temp = this.casesTable.cases.slice();
    if (this.excludePermaPunishments) {
      temp = temp.filter(x => x.modCase.punishedUntil != null || x.modCase.punishmentType === 0 || x.modCase.punishmentType === 2);
    }
    this.showTable = temp;
  }

}
