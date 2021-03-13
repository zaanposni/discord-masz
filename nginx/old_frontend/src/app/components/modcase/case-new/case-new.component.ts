import { HttpParams } from '@angular/common/http';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';
import { ToastrService } from 'ngx-toastr';
import { map } from 'rxjs/operators';
import { AppUser } from 'src/app/models/AppUser';
import { CaseTemplate } from 'src/app/models/CaseTemplate';
import { DiscordUser } from 'src/app/models/DiscordUser';
import { GuildMember } from 'src/app/models/GuildMember';
import { TemplateView } from 'src/app/models/TemplateView';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';
import { CookieTrackerService } from 'src/app/services/cookie-tracker.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-case-new',
  templateUrl: './case-new.component.html',
  styleUrls: ['./case-new.component.scss']
})
export class CaseNewComponent implements OnInit {

  currentUser: AppUser;
  
  showSuggestions: boolean = false;

  loading: boolean = true;

  guildId: string;
  @Input() punishedUntil: string;
  @Input() title: string = '';
  @Input() userid: any;
  @Input() description: string = '';
  @Input() punishment: string = '0';
  @Input() publicNotification: boolean = true;
  @Input() dmNotification: boolean = true;
  @Input() handlePunishment: boolean = true;
  @Input() newLabel: string = '';
  labels: string[] = [];
  fileToUpload!: File | null;
  filesToUpload: File[] = [];

  members: DiscordUser[] = [];
  completeMemberList: DiscordUser[] = [];
  lastMemberPage = 0;

  templatesLoading: boolean = true;
  showTemplates: TemplateView[] = [];
  caseTemplates: TemplateView[] = [];
  templateSearch: string;
  onlyOwnTemplates: boolean = true;

  titleIsInvalid: boolean = false;

  constructor(private toastr: ToastrService, private api: ApiService, private route: ActivatedRoute, private router: Router, public cookieTracker: CookieTrackerService, private auth: AuthService) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid');    
    this.auth.getUserProfile().subscribe((data) => {
      this.currentUser = data;
    });
    let params = new HttpParams()
          .set('partial', 'true');
    this.api.getSimpleData(`/discord/guilds/${this.guildId}/members`, true, params).subscribe((data) => {
      this.completeMemberList = data;
      this.scrollEnd();
    }, (error) => {
      this.toastr.error('Failed to load member list.');
    }, () => {
      this.loading = false;
    });
    this.cookieTracker.currentSettings.subscribe((data) => this.showSuggestions = data.showSuggestions);
    this.reloadTemplates(null);

    let templateId = this.route.snapshot.queryParamMap.get('templateid');
    if (templateId) {
      this.api.getSimpleData(`/templates/${templateId}`).subscribe((data) => {
        this.applyTemplate(data);
      }, () => {
        this.toastr.error('Failed to apply template.', 'Something went wrong');
      })
    }
  }

  reloadTemplates(event: any) {
    this.templatesLoading = true;
    this.showTemplates = [];
    this.api.getSimpleData(`/templatesview`).toPromise().then((data) => {
      this.templatesLoading = false;
      this.caseTemplates = data;
      this.applyTemplateFilter();
    }, () => {
      this.toastr.error('Failed to load templates.', 'Something went wrong');
      this.templatesLoading = false;
    });
  }

  applyTemplateFilter() {
    let filtered = this.caseTemplates;
    if (this.onlyOwnTemplates) {
      filtered = filtered.filter(x => x?.caseTemplate?.userId == this.currentUser?.discordUser?.id);
    }
    if (this.templateSearch) {
      filtered = filtered.filter(x => 
        x?.caseTemplate?.caseTitle?.includes(this.templateSearch) ||
        x?.caseTemplate?.caseDescription?.includes(this.templateSearch) ||
        x?.caseTemplate?.casePunishment?.includes(this.templateSearch) ||
        x?.caseTemplate?.caseLabels?.includes(this.templateSearch) ||
        x?.caseTemplate?.templateName?.includes(this.templateSearch)
      );
    }
    this.showTemplates = filtered;
  }

  applyTemplate(template: CaseTemplate) {

    this.labels = template.caseLabels;
    this.title = template.caseTitle;
    this.description = template.caseDescription;
    this.punishedUntil =  template.casePunishedUntil ? moment.utc(template.casePunishedUntil).toISOString() : null;
    this.dmNotification = template.announceDm;
    this.publicNotification = template.sendPublicNotification;
    this.handlePunishment = template.handlePunishment;

    for (let key in this.punishmentMap) {
      if (this.punishmentMap[key]['punishment'] == template.casePunishment) {
        this.punishment = key;
        break;
      }
    }

    this.toastr.success(`Applied template \'${template.templateName}\'`);
  }

  saveAsTemplate() {
    let visibilities: { [key: string]: string } = {
      '0': 'Everyone',
      '1': 'Only Mods of this guild',
      '2': 'Only me'
    };
    Swal.fire({
      title: 'Choose a visibility',
      text: `Who should see your template?`,
      icon: 'question',
      input: 'select',
      inputOptions: visibilities,
      confirmButtonText: 'Choose',
      showCancelButton: true
    }).then((dialog) => {
      if (dialog.isConfirmed) {
        let visibility = dialog.value;
        Swal.fire({
          title: 'Please name your template',
          icon: 'question',
          input: 'text',
          confirmButtonText: 'Create',
          showCancelButton: true
        }).then((dialog) => {
          if (dialog.isConfirmed) {
            let data = {
              'templatename': dialog.value?.trim() !== '' ? dialog.value : this.title,
              'viewPermission': visibility,
              'title': this.title,
              'description': this.description,
              'punishment': this.punishmentMap[this.punishment]['punishment'],
              'punishedUntil': (this.punishment === '2' || this.punishment === '5') ? new Date(this.punishedUntil).toISOString() : null,
              'punishmentType': this.punishmentMap[this.punishment]['punishmentType'],
              'labels': this.labels,
              'sendPublicNotification': this.publicNotification,
              'handlePunishment': this.handlePunishment,
              'announceDm': this.dmNotification
            }

            let params = new HttpParams()
              .set('guildid', this.guildId);
            this.api.postSimpleData('/templates', data, params).subscribe(() => {
              this.toastr.success('Template created.');
              this.reloadTemplates(null);
            }, () => {
              this.toastr.error('Failed to create template', 'Something went wrong.');
            })
          }
        });
      }
    });
  }

  hideSuggestions() {
    this.cookieTracker.updateCookie('suggestions', 'false');
  }

  scrollEnd() {
    this.members = this.members.concat(this.completeMemberList.slice(this.lastMemberPage * 50, this.lastMemberPage * 50 + 50).filter(x => x.bot == false));
    this.lastMemberPage++;
  }

  onChangeSearch(val: string) {
    this.members = [];
    if (!val) {
      this.lastMemberPage = 0;
      this.scrollEnd();
    } else {
      this.members = this.completeMemberList.filter(x => x.username.toLowerCase().includes(val.toLowerCase())).filter(x => x.bot == false);
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
  };

  handleFileInput(event: any) {
    this.fileToUpload = event.target.files.item(0);
  }

  addFile() {
    console.log(this.fileToUpload);
    if (this.fileToUpload) {
      console.log(this.filesToUpload);
      this.filesToUpload.push(this.fileToUpload);
      console.log(this.filesToUpload);
      this.fileToUpload = null;
    }
    console.log(this.filesToUpload);
  }

  removeFile(file: File) {
    let index = this.filesToUpload.indexOf(file, 0);
    if (index > -1) {
      this.filesToUpload.splice(index, 1);
    }
  }

  uploadFile(file: File, caseId: string) {
    this.api.postFile(`/guilds/${this.guildId}/modcases/${caseId}/files`, file).subscribe((data) => {
      this.toastr.success('File uploaded.');
    }, (error) => {
      this.toastr.error('Cannot upload file.', 'Something went wrong.');
    });
  }

  submitCase() {
    // validation
    if (this.title.trim().length > 100) {
      this.titleIsInvalid = true;
      return;
    }

    // api
    this.loading = true;
    let data = {
      'title': this.title.trim(),
      'description': this.description.trim(),
      'userid': typeof this.userid === "string" ? this.userid.trim() : this.userid['id'],
      'labels': this.labels,
      'punishment': this.punishmentMap[this.punishment]['punishment'],
      'punishmentType': this.punishmentMap[this.punishment]['punishmentType'],
      'punishedUntil': (this.punishment === '2' || this.punishment === '5') ? new Date(this.punishedUntil).toISOString() : null
    };
    let params = new HttpParams()
              .set('sendnotification', this.publicNotification ? 'true' : 'false')
              .set('announceDm', this.dmNotification ? 'true' : 'false')
              .set('handlePunishment', this.handlePunishment ? 'true' : 'false');

    this.api.postSimpleData(`/modcases/${this.guildId}`, data, params).subscribe((data) => {
      let caseId = data['caseid'];
      this.toastr.success(`Case #${caseId} created.`);
      this.filesToUpload.forEach(element => {
        this.uploadFile(element, caseId);
      });
      this.loading = false;
      this.router.navigate(['guilds', this.guildId, 'cases', caseId]);
    }, (error) => {
      this.toastr.error('Cannot create case.', 'Something went wrong.');
      this.loading = false;
    });
  }
}
