import { HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { Observable, ReplaySubject } from 'rxjs';
import { startWith, map } from 'rxjs/operators';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { DiscordUser } from 'src/app/models/DiscordUser';
import { UserMappingView } from 'src/app/models/UserMappingView';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-guild-usermapping',
  templateUrl: './guild-usermapping.component.html',
  styleUrls: ['./guild-usermapping.component.css']
})
export class GuildUsermappingComponent implements OnInit {

  public newMapFormGroup!: FormGroup;
  public filteredMembersA!: Observable<DiscordUser[]>;
  public filteredMembersB!: Observable<DiscordUser[]>;
  public members: ContentLoading<DiscordUser[]> = { loading: true, content: [] };

  private guildId!: string;
  private timeout: any;
  public loading: boolean = true;
  public searchString: string = '';
  private $showUserMappings: ReplaySubject<UserMappingView[]> = new ReplaySubject<UserMappingView[]>(1);
  public showUserMappings: Observable<UserMappingView[]> = this.$showUserMappings.asObservable();
  public allUserMappings: UserMappingView[] = [];
  constructor(private api: ApiService, private toastr: ToastrService, private route: ActivatedRoute, private _formBuilder: FormBuilder, private translator: TranslateService) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid') as string;

    this.resetForm();

    this.filteredMembersA = this.newMapFormGroup.valueChanges
      .pipe(
        startWith(''),
        map(value => this._filter(value.userA))
      );
    this.filteredMembersB = this.newMapFormGroup.valueChanges
      .pipe(
        startWith(''),
        map(value => this._filter(value.userB))
      );

    this.reloadData();
  }

  resetForm() {
    if (this.newMapFormGroup) {
      this.newMapFormGroup.reset();
    }
    this.newMapFormGroup = this._formBuilder.group({
      userA: ['', Validators.required],
      userB: ['', Validators.required],
      reason: ['', Validators.required]
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

  private reloadData() {
    this.loading = true;
    this.$showUserMappings.next([]);
    this.allUserMappings = [];
    this.members = { loading: true, content: [] };

    this.api.getSimpleData(`/guilds/${this.guildId}/usermapview`).subscribe((data: UserMappingView[]) => {
      this.allUserMappings = data;
      this.loading = false;
      this.search();
    }, error => {
      console.error(error);
      this.loading = false;
      this.toastr.error(this.translator.instant('UsermapTable.FailedToLoad'));
    });

    const params = new HttpParams()
          .set('partial', 'true');
    this.api.getSimpleData(`/discord/guilds/${this.guildId}/members`, true, params).subscribe(data => {
      this.members.content = data;
      this.members.loading = false;
    }, error => {
      console.error(error);
      this.toastr.error(this.translator.instant('UsermapTable.FailedToLoadMember'));
    });
  }

  searchChanged(event: any) {
    clearTimeout(this.timeout);
    var $this = this;
    this.timeout = setTimeout(function () {
      if (event.keyCode != 13) {
        $this.search();
      }
    }, 100);
  }

  search() {
    let tempSearch = this.searchString.toLowerCase();
    if (tempSearch.trim()) {
      this.$showUserMappings.next(this.allUserMappings.filter(
        x => x.userMapping.creatorUserId.includes(tempSearch) ||
            x.userMapping.userA.includes(tempSearch) ||
            x.userMapping.userB.includes(tempSearch) ||
            x.userMapping.reason.toLowerCase().includes(tempSearch) ||
            (x.moderator?.username + "#" + x.moderator?.discriminator).toLowerCase().includes(tempSearch) ||
            (x.userA?.username + "#" + x.userA?.discriminator).toLowerCase().includes(tempSearch) ||
            (x.userB?.username + "#" + x.userB?.discriminator).toLowerCase().includes(tempSearch)
      ));
    } else {
      this.$showUserMappings.next(this.allUserMappings);
    }
  }

  removeMap(event: any) {
    this.allUserMappings = this.allUserMappings.filter(x => x.userMapping.id !== event);
    this.search();
  }

  updateEvent() {
    this.reloadData();
  }

  createMap() {
    let data = {
      'userA': this.newMapFormGroup.value.userA,
      'userB': this.newMapFormGroup.value.userB,
      'reason': this.newMapFormGroup.value.reason
    };

    this.api.postSimpleData(`/guilds/${this.guildId}/usermap`, data).subscribe(() => {
      this.reloadData();
      this.toastr.success(this.translator.instant('UsermapTable.Created'));
      this.resetForm();
    }, error => {
      console.error(error);
      this.toastr.error(this.translator.instant('UsermapTable.FailedToCreate'));
    });
  }

}
