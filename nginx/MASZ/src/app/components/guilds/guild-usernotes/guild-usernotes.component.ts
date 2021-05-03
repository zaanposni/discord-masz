import { M, X } from '@angular/cdk/keycodes';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable, ReplaySubject } from 'rxjs';
import { ContentLoading } from 'src/app/models/ContentLoading';
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

  private guildId!: string;
  private timeout: any;
  public loading: boolean = true;
  public searchString: string = '';
  private $showUserNotes: ReplaySubject<UserNoteView[]> = new ReplaySubject<UserNoteView[]>(1);
  public showUserNotes: Observable<UserNoteView[]> = this.$showUserNotes.asObservable();
  public allUserNotes: UserNoteView[] = [];
  constructor(private api: ApiService, private toastr: ToastrService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid') as string;
    this.reloadData();
  }

  private reloadData() {
    this.loading = true;
    this.$showUserNotes.next([]);
    this.allUserNotes = [];
    this.api.getSimpleData(`/guilds/${this.guildId}/usernoteview`).subscribe((data: UserNoteView[]) => {
      this.allUserNotes = data;
      this.loading = false;
      this.search();
    }, () => {
      this.loading = false;
      this.toastr.error("Failed to load usernotes.");
    })
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
  }
}
