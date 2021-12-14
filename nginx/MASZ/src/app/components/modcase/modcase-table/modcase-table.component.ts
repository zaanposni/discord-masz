import { HttpParams } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { Observable, ReplaySubject } from 'rxjs';
import { APIEnumTypes } from 'src/app/models/APIEmumTypes';
import { APIEnum } from 'src/app/models/APIEnum';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { DiscordUser } from 'src/app/models/DiscordUser';
import { IModCaseFilter } from 'src/app/models/IModCaseFilter';
import { ModCaseTable } from 'src/app/models/ModCaseTable';
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

  showTable: ModCaseTable[] = [];
  casesTable: ModCaseTable[] = [];
  isModOrHigher!: Observable<boolean>;
  guildId!: string;
  @Input() uniqueIdentifier: string = "casetable";
  loading: boolean = true;

  @Input() search!: string;
  timeout: any = null;

  // filters
  public members: ReplaySubject<DiscordUser[]> = new ReplaySubject<DiscordUser[]>(1);
  public caseCreationTypes: ReplaySubject<APIEnum[]> = new ReplaySubject<APIEnum[]>(1);
  public punishmentTypes: ReplaySubject<APIEnum[]> = new ReplaySubject<APIEnum[]>(1);

  public userFilterPredicate = (member: DiscordUser, search: string) =>
      `${member.username.toLowerCase()}#${member.discriminator}`.indexOf(search.toLowerCase()) > -1 || member.id == search;
  public userDisplayPredicate = (member: DiscordUser) => `${member.username.toLowerCase()}#${member.discriminator}`;
  public userIdPredicate = (member: DiscordUser) => member.id;

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

    this.enumManager.getEnum(APIEnumTypes.CASECREATIONTYPE).subscribe(data => {
      this.caseCreationTypes.next(data);
    }, error => {
      console.error(error);
      // TODO: this.toastr.error(this.translator.instant('ModCaseTable.FailedToLoad'));
    });
    this.enumManager.getEnum(APIEnumTypes.PUNISHMENT).subscribe(data => {
      this.punishmentTypes.next(data);
    }, error => {
      console.error(error);
      // TODO: this.toastr.error(this.translator.instant('ModCaseTable.FailedToLoad'));
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
    clearTimeout(this.timeout);
    var $this = this;
    this.timeout = setTimeout(function () {
      if (event.keyCode != 13) {
        $this.executeListing(event);
      }
    }, 500);
  }

  private executeListing(value: string) {
    if (value.trim()) {
      this.loading = true;
      this.api.getSimpleData(`/guilds/${this.guildId}/${this.apiUrl}?search=${encodeURIComponent(value)}`).subscribe((data) => {
        this.loading = false;
        this.casesTable = data;
        this.applyCurrentFilters();
      });
    } else {
      this.loadFirstCases();
    }
  }

  selectedMemberChanged(members: DiscordUser[]) {
    this.apiFilter.userIds = members.map(x => x.id);

    let temp = this.apiFilter;
    temp.customTextFilter = encodeURIComponent(temp?.customTextFilter ?? "");
    console.log(JSON.stringify(temp));
    const params = new HttpParams()
          .set('filter', "test")
          .set('search', JSON.stringify(temp));

    console.log(params);
  }

  selectedModChanged(members: DiscordUser[]) {
    this.apiFilter.modIds = members.map(x => x.id);
  }

  selectedCreationTypeChanged(types: APIEnum[]) {
    this.apiFilter.creationTypes = types.map(x => x.key);
  }

  selectedPunishmentTypeChanged(types: APIEnum[]) {
    this.apiFilter.punishmentTypes = types.map(x => x.key);
  }

  loadFirstCases() {
    this.loading = true;
    this.currentPage = 0;
    this.api.postSimpleData(`/guilds/${this.guildId}/${this.apiUrl}`, {}).subscribe(data => {
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
    this.api.postSimpleData(`/guilds/${this.guildId}/${this.apiUrl}`, {}, params).subscribe(data => {
      // this.loading = false;
      data.forEach((element: ModCaseTable ) => {
        this.casesTable.push(element);
      });
      this.applyCurrentFilters();
    }, error => {
      console.error(error);
      this.toastr.error(this.translator.instant('ModCaseTable.FailedToLoad'));
    });
  }

  applyCurrentFilters() {
    let temp = this.casesTable;
    if (this.excludePermaPunishments) {
      temp = temp.filter(x => x.modCase.punishedUntil != null || x.modCase.punishmentType === 0 || x.modCase.punishmentType === 2);
    }
    this.showTable = temp;
  }

}
