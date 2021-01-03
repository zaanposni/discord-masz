import { HttpParams } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-case-new',
  templateUrl: './case-new.component.html',
  styleUrls: ['./case-new.component.scss']
})
export class CaseNewComponent implements OnInit {

  guildId: string;
  @Input() punishedUntil: string;
  @Input() title: string = '';
  @Input() userid: string = '';
  @Input() description: string = '';
  @Input() severity: string = '0';
  @Input() punishment: string = '0';
  @Input() publicNotification: boolean = true;
  @Input() handlePunishment: boolean = true;;
  @Input() newLabel: string = '';
  labels: string[] = [];

  constructor(private toastr: ToastrService, private api: ApiService, private route: ActivatedRoute, private router: Router) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid');
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

  punishmentMap: any = {
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

  submitCase() {
    let data = {
      'title': this.title,
      'description': this.description,
      'userid': this.userid,
      'severity': this.severity,
      'labels': this.labels,
      'punishment': this.punishmentMap[this.punishment]['punishment'],
      'punishmentType': this.punishmentMap[this.punishment]['punishmentType'],
      'punishedUntil': (this.punishment === '2' || this.punishment === '5') ? new Date(this.punishedUntil).toISOString() : null
    };
    let params = new HttpParams()
              .set('sendnotification', this.publicNotification ? 'true' : 'false')
              .set('handlePunishment', this.handlePunishment ? 'true' : 'false');;

    this.api.postSimpleData(`/modcases/${this.guildId}`, data, params).subscribe((data) => {
      this.toastr.success('Case created.');
      this.router.navigate(['guilds', this.guildId, 'cases', data['caseid']]);
    }, (error) => {
      this.toastr.error('Cannot create case.', 'Something went wrong.');
    })
  }
}
