import { M, X } from '@angular/cdk/keycodes';
import { HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { Observable, ReplaySubject } from 'rxjs';
import { startWith, map } from 'rxjs/operators';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { DiscordUser } from 'src/app/models/DiscordUser';
import { UserNote } from 'src/app/models/UserNote';
import { UserNoteDto } from 'src/app/models/UserNoteDto';
import { UserNoteView } from 'src/app/models/UserNoteView';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-guild-usernotes',
  templateUrl: './guild-usernotes.component.html',
  styleUrls: ['./guild-usernotes.component.css']
})
export class GuildUsernotesComponent implements OnInit {

  public newNoteFormGroup!: FormGroup;
  public filteredMembers!: Observable<DiscordUser[]>;
  public members: ContentLoading<DiscordUser[]> = { loading: true, content: [] };

  private guildId!: string;
  private timeout: any;
  public loading: boolean = true;
  public searchString: string = '';
  private $showUserNotes: ReplaySubject<UserNoteView[]> = new ReplaySubject<UserNoteView[]>(1);
  public showUserNotes: Observable<UserNoteView[]> = this.$showUserNotes.asObservable();
  public allUserNotes: UserNoteView[] = [];
  constructor(private api: ApiService, private toastr: ToastrService, private route: ActivatedRoute, private _formBuilder: FormBuilder, private translator: TranslateService) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid') as string;

    this.resetForm();

    this.filteredMembers = this.newNoteFormGroup.valueChanges
      .pipe(
        startWith(''),
        map(value => this._filter(value.userid))
      );

    this.reloadData();
  }

  resetForm() {
    if (this.newNoteFormGroup) {
      this.newNoteFormGroup.reset();
    }
    this.newNoteFormGroup = this._formBuilder.group({
      userid: ['', Validators.required],
      description: ['', Validators.required]
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
    this.$showUserNotes.next([]);
    this.allUserNotes = [];
    this.members = { loading: true, content: [] };

    this.api.getSimpleData(`/guilds/${this.guildId}/usernoteview`).subscribe((data: UserNoteView[]) => {
      this.allUserNotes = data;
      this.loading = false;
      this.search();
    }, error => {
      console.error(error);
      this.loading = false;
      this.toastr.error(this.translator.instant('UsernoteTable.FailedToLoad'));
    });

    const params = new HttpParams()
          .set('partial', 'true');
    this.api.getSimpleData(`/discord/guilds/${this.guildId}/members`, true, params).subscribe(data => {
      this.members.content = data;
      this.members.loading = false;
    }, error => {
      console.error(error);
      this.toastr.error(this.translator.instant('UsernoteTable.FailedToLoadMember'));
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
      this.$showUserNotes.next(this.allUserNotes.filter(
        x => x.userNote.creatorId.includes(tempSearch) ||
            x.userNote.userId.includes(tempSearch) ||
            x.userNote.description.toLowerCase().includes(tempSearch) ||
            (x.moderator?.username + "#" + x.moderator?.discriminator).toLowerCase().includes(tempSearch) ||
            (x.user?.username + "#" + x.user?.discriminator).toLowerCase().includes(tempSearch)
      ));
    } else {
      this.$showUserNotes.next(this.allUserNotes);
    }
  }

  removeNote(event: any) {
    this.allUserNotes = this.allUserNotes.filter(x => x.userNote.id !== event);
    this.search();
  }

  updateEvent() {
    this.reloadData();
  }

  createNote() {
    let data = {
      'userid': this.newNoteFormGroup.value.userid,
      'description': this.newNoteFormGroup.value.description
    };

    this.api.putSimpleData(`/guilds/${this.guildId}/usernote`, data).subscribe(() => {
      this.reloadData();
      this.resetForm();
      this.toastr.success(this.translator.instant('UsernoteTable.Created'));
    }, error => {
      console.error(error);
      this.toastr.error(this.translator.instant('UsernoteTable.FailedToCreate'));
    });
  }
}
