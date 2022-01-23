import { HttpParams } from '@angular/common/http';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { Observable, ReplaySubject, Subject } from 'rxjs';
import { APIEnumTypes } from 'src/app/models/APIEmumTypes';
import { APIEnum } from 'src/app/models/APIEnum';
import { AppUser } from 'src/app/models/AppUser';
import { CaseComment } from 'src/app/models/CaseComment';
import { CaseDeleteDialogData } from 'src/app/models/CaseDeleteDialogData';
import { CaseView } from 'src/app/models/CaseView';
import { CommentEditDialog } from 'src/app/models/CommentEditDialog';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { FileInfo } from 'src/app/models/FileInfo';
import { Guild } from 'src/app/models/Guild';
import { convertModcaseToPunishmentString, ModCase } from 'src/app/models/ModCase';
import { PunishmentType } from 'src/app/models/PunishmentType';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';
import { EnumManagerService } from 'src/app/services/enum-manager.service';
import { CaseDeleteDialogComponent } from '../../dialogs/case-delete-dialog/case-delete-dialog.component';
import { CommentEditDialogComponent } from '../../dialogs/comment-edit-dialog/comment-edit-dialog.component';
import { ConfirmationDialogComponent } from '../../dialogs/confirmation-dialog/confirmation-dialog.component';
import * as moment from 'moment';

@Component({
  selector: 'app-modcase-view',
  templateUrl: './modcase-view.component.html',
  styleUrls: ['./modcase-view.component.css']
})
export class ModcaseViewComponent implements OnInit {
  public convertModcaseToPunishmentString = convertModcaseToPunishmentString;
  public restoringCase: boolean = false;

  public guildId!: string;
  public caseId!: string;

  @ViewChild("fileInput", {static: false}) fileInput!: ElementRef;
  public filesToUpload: any[] = [];
  public newComment!: string;

  public showActivationSlider = false;
  public activationSliderValue = false;
  public activationSliderModeDeactivation = true;
  public punishmentDescriptionTranslationKey = "";
  public isModOrHigher: boolean = false;
  public renderedDescription!: string;
  private filesSubject$ = new ReplaySubject<FileInfo>(1);
  public files: ContentLoading<Observable<FileInfo>> = { loading: true, content: this.filesSubject$.asObservable() };
  public currentUser: ContentLoading<AppUser> = { loading: true, content: undefined };
  public currentGuild: ContentLoading<Guild> = { loading: true, content: undefined };
  public modCase: ContentLoading<CaseView> = { loading: true, content: undefined };
  public punishments: ContentLoading<APIEnum[]> = { loading: true, content: [] };
  public punishment: string = "Unknown";

  public creationTypes: APIEnum[] = [];
  public creationType = "";

  public markedDeleteParams = {
    user: ""
  };
  public lockedCommentsParams = {
    user: ""
  };

  constructor(private route: ActivatedRoute, private auth: AuthService, private toastr: ToastrService, private api: ApiService, public router: Router, private _formBuilder: FormBuilder, private dialog: MatDialog, private enumManager: EnumManagerService, private translator: TranslateService) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid') as string;
    this.caseId = this.route.snapshot.paramMap.get('caseid') as string;

    // reload files from case creation
    if (this.route.snapshot.queryParamMap.get('reloadfiles') !== '0' && this.route.snapshot.queryParamMap.get('reloadfiles') != null) {
      var $this = this;
      setTimeout(function() {
        $this.reloadFiles();
      }, 5000);
      setTimeout(function() {
        $this.reloadFiles();
      }, 10000);
    }

    this.auth.isModInGuild(this.guildId).subscribe((data) => { this.isModOrHigher = data; });
    this.auth.getUserProfile().subscribe((data) => { this.currentUser = { loading: false, content: data }; });

    this.reload();
  }

  private reload() {
    this.reloadCase();
    this.reloadGuild();
    this.reloadPunishmentEnum();
    this.reloadFiles();
    this.reloadCreationTypes();
  }

  private reloadCreationTypes() {
    this.enumManager.getEnum(APIEnumTypes.CASECREATIONTYPE).subscribe((data) => {
      this.creationTypes = data;
      this.creationType = data?.find(x => x.key == this.modCase.content?.modCase?.creationType)?.value ?? '';
    }, error => {
      console.error(error);
    });
  }

  private reloadPunishmentEnum() {
    this.punishments = { loading: true, content: [] };
    this.enumManager.getEnum(APIEnumTypes.PUNISHMENT).subscribe(data => {
      this.punishments.loading = false;
      this.punishments.content = data;
      this.punishment = convertModcaseToPunishmentString(this.modCase.content?.modCase, this.punishments?.content);
    }, error => {
      console.error(error);
      this.punishments.loading = false;
      this.toastr.error(this.translator.instant('ModCaseView.FailedToLoad.Punishments'));
    });
  }

  private reloadFiles() {
    this.files.loading = true;
    this.api.getSimpleData(`/guilds/${this.guildId}/cases/${this.caseId}/files`).subscribe(data => {
      this.filesSubject$.next(data);
      this.files.loading = false;
    }, error => {
      console.error(error);
      this.files.loading = false;
      this.toastr.error(this.translator.instant('ModCaseView.FailedToLoad.Files'));
    });
  }

  public redirectToUserscan(userId: any) {
    if (this.isModOrHigher) {
      let params: Params = {
        'search': userId
      }
      this.router.navigate(['scanning'], { queryParams: params });
    }
  }

  private reloadGuild() {
    this.currentGuild = { loading: true, content: undefined };
    this.api.getSimpleData(`/discord/guilds/${this.guildId}`).subscribe((data) => {
      this.currentGuild.content = data;
      this.currentGuild.loading = false;
    }, () => {
      this.currentGuild.loading = false;
      this.toastr.error(this.translator.instant('ModCaseView.FailedToLoad.Guild'));
    });
  }

  private reloadCase() {
    this.modCase = { loading: true, content: undefined };
    this.punishmentDescriptionTranslationKey = "";
    this.api.getSimpleData(`/guilds/${this.guildId}/cases/${this.caseId}/view`).subscribe((data: CaseView) => {
      this.modCase.content = data;
      this.renderedDescription = this.renderDescription(data.modCase.description, this.guildId)
      this.punishment = convertModcaseToPunishmentString(this.modCase.content?.modCase, this.punishments?.content);
      this.creationType = this.creationTypes?.find(x => x.key == this.modCase.content?.modCase?.creationType)?.value ?? '';
      this.markedDeleteParams = {
        user: data.deletedBy ? `${data.deletedBy.username}#${data.deletedBy.discriminator}` : this.translator.instant('ModCaseView.Moderators')
      };
      this.lockedCommentsParams = {
        user: data.lockedBy ? `${data.lockedBy.username}#${data.lockedBy.discriminator}` : this.translator.instant('ModCaseView.Moderators')
      };
      if (this.modCase.content.modCase?.punishedUntil === null || moment(this.modCase.content.modCase?.punishedUntil).utc(true).isAfter(moment())) {
        this.showActivationSlider = true;
        this.activationSliderModeDeactivation = this.modCase.content.modCase?.punishmentActive;
      }
      if (this.modCase.content.modCase.punishmentType !== PunishmentType.None && this.modCase.content.modCase.punishmentType !== PunishmentType.Kick && ! this.modCase.content.modCase.punishmentActive) {
        if (this.modCase.content.modCase?.punishedUntil == null) {
          this.punishmentDescriptionTranslationKey = "CaseInactive";
        } else if (moment(this.modCase.content.modCase?.punishedUntil).utc(true).isAfter(moment())) {
          this.punishmentDescriptionTranslationKey = "CaseInactive";
        } else {
          this.punishmentDescriptionTranslationKey = "PunishmentExpired";
        }
      }
      this.modCase.loading = false;
    }, error => {
      console.error(error);
      this.modCase.loading = false;
      this.toastr.error(this.translator.instant('ModCaseView.FailedToLoad.Case'));
    });
  }

  public handleActivation() {
    const confirmDialogRef = this.dialog.open(ConfirmationDialogComponent);
    confirmDialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        if (this.activationSliderModeDeactivation) {
          this.api.deleteData(`/guilds/${this.guildId}/cases/${this.caseId}/active`).subscribe(() => {
            this.toastr.success(this.translator.instant('ModCaseView.Deactivated.Success'));
            this.activationSliderValue = false;
            this.reloadCase();
          }, error => {
            console.error(error);
            this.activationSliderValue = false;
            this.toastr.error(this.translator.instant('ModCaseView.Deactivated.Failed'));
          });
        } else {
          this.api.postSimpleData(`/guilds/${this.guildId}/cases/${this.caseId}/active`, {}).subscribe(() => {
            this.toastr.success(this.translator.instant('ModCaseView.Activated.Success'));
            this.activationSliderValue = false;
            this.reloadCase();
          }, error => {
            console.error(error);
            this.activationSliderValue = false;
            this.toastr.error(this.translator.instant('ModCaseView.Activated.Failed'));
          });
        }
      } else {
        this.activationSliderValue = false;
      }
    });
  }

  deleteCase() {
    const caseDeleteConfig: CaseDeleteDialogData = {
      case: this.modCase.content?.modCase as ModCase,
      sendNotification: false,
      isAdmin: this.currentUser.content?.isAdmin ?? false,
      forceDelete: false
    };
    const confirmDialogRef = this.dialog.open(CaseDeleteDialogComponent, {
      data: caseDeleteConfig
    });
    confirmDialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        const params = new HttpParams()
          .set("sendnotification", caseDeleteConfig.sendNotification ? 'true' : 'false')
          .set('forceDelete', caseDeleteConfig.forceDelete ? 'true' : 'false');

        this.api.deleteData(`/guilds/${this.guildId}/cases/${this.caseId}`, params).subscribe(() => {
          if (caseDeleteConfig.forceDelete) {
            this.toastr.success(this.translator.instant('ModCaseView.DeleteCase.ForceDeleted'));
            this.router.navigate(['guilds', this.guildId]);
          } else {
            this.toastr.success(this.translator.instant('ModCaseView.DeleteCase.Deleted'));
            this.reloadCase();
          }
        }, error => {
          console.error(error);
          this.toastr.error(this.translator.instant('ModCaseView.DeleteCase.Failed'));
        });
      }
    });
  }

  uploadInit() {
    const fileInput = this.fileInput.nativeElement;
    this.filesToUpload = [];
    fileInput.onchange = () => {
      for (let index = 0; index < fileInput.files.length; index++)
      {
        const file = fileInput.files[index];
        this.filesToUpload.push({ data: file, inProgress: false, progress: 0});
      }
      this.uploadFiles();
    };
    fileInput.click();
  }

  userNoteDeleted() {
    if (this.modCase.content) {
      this.modCase.content.userNote = undefined;
    }
  }

  restoreCase() {
    this.restoringCase = true;
    this.api.deleteData(`/guilds/${this.guildId}/bin/${this.caseId}/restore`).subscribe(() => {
      this.toastr.success(this.translator.instant('ModCaseView.RestoreCase.Restored'));
      this.reloadCase();
      this.restoringCase = false;
    }, error => {
      console.error(error);
      this.toastr.error(this.translator.instant('ModCaseView.RestoreCase.Failed'));
      this.restoringCase = false;
    });
  }

  deleteCaseFromBin() {
    this.restoringCase = true;
    this.api.deleteData(`/guilds/${this.guildId}/bin/${this.caseId}/delete`).subscribe(() => {
      this.toastr.success(this.translator.instant('ModCaseView.DeleteCase.ForceDeleted'));
      this.router.navigate(['guilds', this.guildId]);
    }, error => {
      console.error(error);
      this.toastr.error(this.translator.instant('ModCaseView.DeleteCase.Failed'));
      this.restoringCase = false;
    });
  }

  uploadFiles() {
    for (let file of this.filesToUpload.map(x => x.data)) {
      this.api.postFile(`/guilds/${this.guildId}/cases/${this.caseId}/files`, file).subscribe(() => {
        this.toastr.success(this.translator.instant('ModCaseView.FileUpload.Uploaded'));
        this.reloadFiles();
      }, error => {
        console.error(error);
        this.toastr.error(this.translator.instant('ModCaseView.FileUpload.Failed'));
      });
    }
  }

  deleteFile(filename: string) {
    this.api.deleteData(`/guilds/${this.guildId}/cases/${this.caseId}/files/${filename}`).subscribe(() => {
      this.toastr.success(this.translator.instant('ModCaseView.FileDelete.Deleted'));
      this.reloadFiles();
    }, error => {
      console.error(error);
      this.toastr.error(this.translator.instant('ModCaseView.FileDeleted.Failed'));
    });
  }

  deleteComment(commentId: number) {
    this.api.deleteData(`/guilds/${this.guildId}/cases/${this.caseId}/comments/${commentId}`).subscribe(() => {
      this.toastr.success(this.translator.instant('ModCaseView.CommentDelete.Deleted'));
      this.reloadCase();
    }, error => {
      console.error(error);
      this.toastr.error(this.translator.instant('ModCaseView.CommentDelete.Failed'));
    });
  }

  updateComment(comment: CaseComment) {
    const commentDialog: CommentEditDialog = {
      message: comment.message,
    };
    const editDialogRef = this.dialog.open(CommentEditDialogComponent, {
      data: commentDialog
    });
    editDialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        const data = {
          "message": commentDialog.message
        }
        this.api.putSimpleData(`/guilds/${this.guildId}/cases/${this.caseId}/comments/${comment.id}`, data).subscribe(() => {
          this.toastr.success(this.translator.instant('ModCaseView.CommentUpdate.Updated'));
          this.reloadCase();
        }, error => {
          console.error(error);
          this.toastr.error(this.translator.instant('ModCaseView.CommentUpdate.Failed'));
        });
      }
    });
    this.reloadCase();
  }

  postComment() {
    const data = {
      "message": this.newComment.trim()
    };

    this.api.postSimpleData(`/guilds/${this.guildId}/cases/${this.caseId}/comments`, data, undefined, true, true).subscribe(() => {
      this.newComment = "";
      this.toastr.success(this.translator.instant('ModCaseView.CommentPost.Posted'));
      this.reloadCase();
    }, error => {
      console.error(error);
      this.toastr.error(this.translator.instant('ModCaseView.CommentPost.Failed'));
    });
  }

  lockCaseComments() {
    this.api.postSimpleData(`/guilds/${this.guildId}/cases/${this.caseId}/lock`, null).subscribe(() => {
      this.toastr.success(this.translator.instant('ModCaseView.CommentLock.Locked'));
      this.reloadCase();
    }, error => {
      console.error(error);
      this.toastr.error(this.translator.instant('ModCaseView.CommentLock.Failed'));
    });
  }

  unlockCaseComments() {
    this.api.deleteData(`/guilds/${this.guildId}/cases/${this.caseId}/lock`).subscribe(() => {
      this.toastr.success(this.translator.instant('ModCaseView.CommentUnlock.Unlocked'));
      this.reloadCase();
    }, error => {
      console.error(error);
      this.toastr.error(this.translator.instant('ModCaseView.CommentUnlock.Failed'));
    });
  }

  safe_tags_replace(str: any) {
    return str.replace(/[&<>]/g, replaceTag);
  }

  renderDescription(str: string, guildId: string): string {
    return this.safe_tags_replace(str).replace(/#(\d+)/g, function(match: any, id: any) {
      return `<a href="/guilds/${guildId}/cases/${id}">#${id}</a>`
    });  // TODO: make this a routerLink?
  }
}

const tagsToReplace: { [key: string]: string } = {
  '&': '&amp;',
  '<': '&lt;',
  '>': '&gt;'
};

function replaceTag(tag: string) {
  return tagsToReplace[tag] || tag;
}
