import { HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Moment } from 'moment';
import { ToastrService } from 'ngx-toastr';
import { Observable, ReplaySubject } from 'rxjs';
import { APIEnumTypes } from 'src/app/models/APIEmumTypes';
import { APIEnum } from 'src/app/models/APIEnum';
import { DiscordUser } from 'src/app/models/DiscordUser';
import { IAppealFilter } from 'src/app/models/IAppealFilter';
import { IAppealStructure } from 'src/app/models/IAppealStructure';
import { IAppealTable } from 'src/app/models/IAppealTable';
import { IAppealView } from 'src/app/models/IAppealView';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';
import { EnumManagerService } from 'src/app/services/enum-manager.service';

@Component({
  selector: 'app-guild-appeals-table',
  templateUrl: './guild-appeals-table.component.html',
  styleUrls: ['./guild-appeals-table.component.css']
})
export class GuildAppealsTableComponent implements OnInit {

  structures: IAppealStructure[] = [];
  currentPage: number = 0;
  appealTable!: IAppealTable;
  $isAllowedToCreateNewAppeal: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);
  isAllowedToCreateNewAppeal: Observable<boolean> = this.$isAllowedToCreateNewAppeal.asObservable();
  isMember!: Observable<boolean>;
  isAdminOrHigher!: Observable<boolean>;
  guildId!: string;
  loading: boolean = true;

  // filters
  public members: ReplaySubject<DiscordUser[]> = new ReplaySubject<DiscordUser[]>(1);
  public appealStatus: ReplaySubject<APIEnum[]> = new ReplaySubject<APIEnum[]>(1);

  public editStatusCtrl: FormControl = new FormControl();

  public userFilterPredicate = (member: DiscordUser, search: string) =>
      `${member.username.toLowerCase()}#${member.discriminator}`.indexOf(search.toLowerCase()) > -1 || member.id == search;
  public userDisplayPredicate = (member: DiscordUser) => `${member.username.toLowerCase()}#${member.discriminator}`;
  public userIdPredicate = (member: DiscordUser) => member.id;
  public userComparePredicate = (member: DiscordUser, member2: DiscordUser) => member?.id == member2?.id;

  public enumFilterPredicate = (enumType: APIEnum, search: string) =>
      `${enumType.value.toLowerCase()}`.indexOf(search.toLowerCase()) > -1 || enumType.key.toString() == search;
  public enumDisplayPredicate = (enumType: APIEnum) => enumType.value;

  public apiFilter: IAppealFilter = {};

  constructor(public router: Router, private api: ApiService, private auth: AuthService, private route: ActivatedRoute, private toastr: ToastrService, private translator: TranslateService, private enumManager: EnumManagerService) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe((data) => {
      this.guildId = data.get('guildid') as string;
      this.isMember = this.auth.isMemberInGuild(this.guildId);
      this.isAdminOrHigher = this.auth.isAdminInGuild(this.guildId);
      this.$isAllowedToCreateNewAppeal.next(false);
      this.reload();
    });

    this.enumManager.getEnum(APIEnumTypes.APPEALSTATUS).subscribe(data => {
      this.appealStatus.next(data);
    });
  }

  reload() {
    this.loadFirstCases();
    this.loadMembers();

    this.api.getSimpleData(`/guilds/${this.guildId}/appeal/allowed`).subscribe((data: { allowed: boolean }) => {
      this.$isAllowedToCreateNewAppeal.next(data.allowed);
    });

    this.api.getSimpleData(`/guilds/${this.guildId}/appealstructures`).subscribe((data: IAppealStructure[]) => {
      this.structures = data;
    });
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

  selectedSinceChanged(date: Moment) {
    this.apiFilter.since = date?.toISOString();
  }
  selectedUntilChanged(date: Moment) {
    this.apiFilter.before = date?.toISOString();
  }
  selectedMemberChanged(members: DiscordUser[]) {
    this.apiFilter.userIds = members?.map(x => x.id) ?? [];
  }
  selectedStatusChanged(type: APIEnum[]) {
    this.apiFilter.status = type?.map(x => x.key) ?? [];
  }

  loadFirstCases() {
    this.loading = true;
    this.currentPage = 0;
    this.api.postSimpleData(`/guilds/${this.guildId}/appeal/table`, this.apiFilter).subscribe((data: IAppealTable) => {
      this.loading = false;
      this.appealTable = data;
    }, error => {
      this.loading = false;
      console.error(error);
      this.toastr.error(this.translator.instant('AppealTable.FailedToLoad'));
    });
  }

  loadNextPage() {
    // this.loading = true;
    this.currentPage++;
    const params = new HttpParams()
          .set('startPage', this.currentPage.toString());
    this.api.postSimpleData(`/guilds/${this.guildId}/appeal/table`, {}, params).subscribe((data: IAppealTable) => {
      // this.loading = false;
      this.appealTable.appealViews.push(...data.appealViews);
      this.appealTable.fullSize = data.fullSize;
    }, error => {
      console.error(error);
      this.toastr.error(this.translator.instant('AppealTable.FailedToLoad'));
    });
  }
}
