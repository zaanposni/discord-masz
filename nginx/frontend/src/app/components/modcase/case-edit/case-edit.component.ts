import { HttpParams } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';
import { ToastrService } from 'ngx-toastr';
import { DiscordUser } from 'src/app/models/DiscordUser';
import { GuildMember } from 'src/app/models/GuildMember';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-case-edit',
  templateUrl: './case-edit.component.html',
  styleUrls: ['./case-edit.component.scss']
})
export class CaseEditComponent implements OnInit {

  guildId: string;
  caseId: string;
  @Input() punishedUntil: string;
  @Input() title: string = '';
  @Input() userid: any;
  @Input() description: string = '';
  @Input() punishment: string = '0';
  @Input() publicNotification: boolean = true;
  @Input() handlePunishment: boolean = true;;
  @Input() newLabel: string = '';
  labels: string[] = [];
  members: DiscordUser[] = [];
  completeMemberList: GuildMember[] = [];
  lastMemberPage = 0;

  constructor(private toastr: ToastrService, private api: ApiService, private route: ActivatedRoute, private router: Router) { }

  punishmentMap: { [key: string]: { [k: string]: any } } = {
    '0' : {
      'punishment': 'Warn',
      'punishmentType': 0
    },
    '1' : {
      'punishment': 'Mute',
      'punishmentType': 1
    },
    '2' : {
      'punishment': 'TempMute',
      'punishmentType': 1
    },
    '3' : {
      'punishment': 'Kick',
      'punishmentType': 2
    },
    '4' : {
      'punishment': 'Ban',
      'punishmentType': 3
    },
    '5' : {
      'punishment': 'TempBan',
      'punishmentType': 3
    },
    '6' : {
      'punishment': 'Notice',
      'punishmentType': 0
    },
    '7' : {
      'punishment': 'None',
      'punishmentType': 0
    }
  } 

  getUTCDate(dateString: string): string {
    // dateString format will be "YYYY-MM-DDThh:mm:ss"
    
    let date: any;
    let time: any;
    
    let year: any;
    let month: any;
    let day: any;

    let hours: any;
    let minutes: any;
    let seconds: any;

    [date, time] = dateString.split("T");
    [year, month, day] = date.split("-");
    [hours, minutes, seconds] = time.split(":");
    // month is 0 indexed in Date operations, subtract 1 when converting string to Date object
    return new Date(Date.UTC(year, month - 1, day, hours, minutes, seconds)).toISOString();
  }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid');
    this.caseId = this.route.snapshot.paramMap.get('caseid');
    this.api.getSimpleData(`/modcases/${this.guildId}/${this.caseId}`).subscribe((data) => {
      this.punishedUntil = data['punishedUntil'] ? moment.utc(data['punishedUntil']).toISOString() : null;
      this.title = data['title'];
      this.userid = data['userId'];
      this.description = data['description'];
      this.labels = data['labels'];
      for (let key in this.punishmentMap) {
        if (this.punishmentMap[key]['punishment'] == data['punishment']) {
          this.punishment = key;
          break;
        }
      }
    }, (error) => {
      this.toastr.error("Failed to load current modcase.");
      this.router.navigate(['guilds', this.guildId]);
    });
    this.api.getSimpleData(`/discord/guilds/${this.guildId}/members`).subscribe((data) => {
      this.completeMemberList = data;
      this.scrollEnd();
    });
  }

  scrollEnd() {
    this.members = this.members.concat(this.completeMemberList.slice(this.lastMemberPage * 50, this.lastMemberPage * 50 + 50).map(x => x.user).filter(x => x.bot == false));
    this.lastMemberPage++;
  }

  onChangeSearch(val: string) {
    this.members = [];
    if (!val) {
      this.lastMemberPage = 0;
      this.scrollEnd();
    } else {
      this.members = this.completeMemberList.filter(x => x.user.username.toLowerCase().includes(val.toLowerCase())).map(x => x.user).filter(x => x.bot == false);
    }
  }

  addLabel() {
    if (this.newLabel && this.newLabel.trim()) {
      if (this.labels.indexOf(this.newLabel.trim()) > -1) {
        this.toastr.warning("You have already added this label.");
      } else {
        this.labels.push(this.newLabel.trim());
        this.newLabel = "";
      }
    }
  }

  deleteLabel(label: string) {
    let index = this.labels.indexOf(label, 0);
    if (index > -1) {
      this.labels.splice(index, 1);
    }
  }

  submitCase() {
    let data = {
      'title': this.title.trim(),
      'description': this.description.trim(),
      'userid': typeof this.userid === "string" ? this.userid.trim() : this.userid['id'],
      'labels': this.labels,
      'punishment': this.punishmentMap[this.punishment]['punishment'],
      'punishmentType': this.punishmentMap[this.punishment]['punishmentType'],
      'punishedUntil': (this.punishment === '2' || this.punishment === '5') ? moment.utc(this.punishedUntil).toISOString() : null
    };
    let params = new HttpParams()
              .set('sendnotification', this.publicNotification ? 'true' : 'false')
              .set('handlePunishment', this.handlePunishment ? 'true' : 'false');;

    this.api.putSimpleData(`/modcases/${this.guildId}/${this.caseId}`, data, params).subscribe((data) => {
      this.toastr.success('Case updated.');
      this.router.navigate(['guilds', this.guildId, 'cases', data['caseid']]);
    }, (error) => {
      this.toastr.error('Cannot update case.', 'Something went wrong.');
    })
  }
}
