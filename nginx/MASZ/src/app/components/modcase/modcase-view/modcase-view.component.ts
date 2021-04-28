import { HttpParams } from '@angular/common/http';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable, ReplaySubject, Subject } from 'rxjs';
import { AppUser } from 'src/app/models/AppUser';
import { CaseComment } from 'src/app/models/CaseComment';
import { CaseDeleteDialogData } from 'src/app/models/CaseDeleteDialogData';
import { CaseView } from 'src/app/models/CaseView';
import { CommentEditDialog } from 'src/app/models/CommentEditDialog';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { FileInfo } from 'src/app/models/FileInfo';
import { Guild } from 'src/app/models/Guild';
import { ModCase } from 'src/app/models/ModCase';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';
import { CaseDeleteDialogComponent } from '../../dialogs/case-delete-dialog/case-delete-dialog.component';
import { CommentEditDialogComponent } from '../../dialogs/comment-edit-dialog/comment-edit-dialog.component';

@Component({
  selector: 'app-modcase-view',
  templateUrl: './modcase-view.component.html',
  styleUrls: ['./modcase-view.component.css']
})
export class ModcaseViewComponent implements OnInit {

  public restoringCase: boolean = false;

  public guildId!: string;
  public caseId!: string;

  @ViewChild("fileInput", {static: false}) fileInput!: ElementRef;
  public filesToUpload: any[] = [];
  public newComment!: string;

  public isModOrHigher: boolean = false;
  public renderedDescription!: string;
  private filesSubject$ = new ReplaySubject<FileInfo>(1);
  public files: ContentLoading<Observable<FileInfo>> = { loading: true, content: this.filesSubject$.asObservable() };
  public currentUser: ContentLoading<AppUser> = { loading: true, content: undefined };
  public currentGuild: ContentLoading<Guild> = { loading: true, content: undefined };
  public modCase: ContentLoading<CaseView> = { loading: true, content: undefined };
  constructor(private route: ActivatedRoute, private auth: AuthService, private toastr: ToastrService, private api: ApiService, public router: Router, private _formBuilder: FormBuilder, private dialog: MatDialog) { }
  
  
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
    this.reloadFiles();
  }

  private reloadFiles() {
    this.files.loading = true;
    this.api.getSimpleData(`/guilds/${this.guildId}/modcases/${this.caseId}/files`).subscribe((data) => {
      this.filesSubject$.next(data);
      this.files.loading = false;
    }, () => {
      this.files.loading = false;
      this.toastr.error("Failed to load files.");
    });
  }

  public redirectToUserscan(userId: any) {
    if (this.isModOrHigher) {
      let params: Params = {
        'userid': userId
      }
      this.router.navigate(['userscan'], { queryParams: params });
    }
  }

  private reloadGuild() {
    this.currentGuild = { loading: true, content: undefined };
    this.api.getSimpleData(`/discord/guilds/${this.guildId}`).subscribe((data) => {
      this.currentGuild.content = data;
      this.currentGuild.loading = false;
    }, () => {
      this.currentGuild.loading = false;
      this.toastr.error("Failed to load guild info.");
    });
  }

  private reloadCase() {
    this.modCase = { loading: true, content: undefined };
    this.api.getSimpleData(`/guilds/${this.guildId}/modcases/${this.caseId}/view`).subscribe((data) => {
      this.modCase.content = data;
      this.renderedDescription = this.renderDescription(data.modCase.description, this.guildId)
      this.modCase.loading = false;
    }, () => {
      this.modCase.loading = false;
      this.toastr.error("Failed to load case.");
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

        this.api.deleteData(`/modcases/${this.guildId}/${this.caseId}`, params).subscribe(() => {
          if (caseDeleteConfig.forceDelete) {
            this.toastr.success("Case deleted.");
            this.router.navigate(['guilds', this.guildId]);
          } else {
            this.toastr.success("Case marked to be deleted.");
            this.reloadCase();
          }
        }, () => {
          this.toastr.error("Failed to delete case.");
        });
      }
    });
  }

  uploadInit() {
    const fileInput = this.fileInput.nativeElement;
    fileInput .onchange = () => {
      for (let index = 0; index < fileInput .files.length; index++)
      {
        const file = fileInput .files[index];
        this.filesToUpload.push({ data: file, inProgress: false, progress: 0});
      }
      this.uploadFiles();
    };
    fileInput.click();
  }

  restoreCase() {
    this.restoringCase = true;
    this.api.deleteData(`/guilds/${this.guildId}/bin/${this.caseId}/restore`).subscribe((data) => {
      this.toastr.success("Restored case.");
      this.reloadCase();
      this.restoringCase = false;
    }, () => {
      this.toastr.error("Failed to restore case.");
      this.restoringCase = false;
    });
  }

  deleteCaseFromBin() {
    this.restoringCase = true;
    this.api.deleteData(`/guilds/${this.guildId}/bin/${this.caseId}/delete`).subscribe((data) => {
      this.toastr.success("Case deleted.");
      this.router.navigate(['guilds', this.guildId]);
    }, () => {
      this.toastr.error("Failed to delete case.");
      this.restoringCase = false;
    });
  }

  uploadFiles() {
    for (let file of this.filesToUpload.map(x => x.data)) {
      this.api.postFile(`/guilds/${this.guildId}/modcases/${this.caseId}/files`, file).subscribe((data) => {
        this.toastr.success('File uploaded.');
        this.reloadFiles();
      }, (error) => {
        this.toastr.error('Failed to upload file.');
      });
    }
  }

  deleteFile(filename: string) {
    this.api.deleteData(`/guilds/${this.guildId}/modcases/${this.caseId}/files/${filename}`).subscribe((data) => {
      this.toastr.success("File deleted.");
      this.reloadFiles();
    }, () => {
      this.toastr.error("Failed to delete file.");
    });
  }

  deleteComment(commentId: number) {
    this.api.deleteData(`/modcases/${this.guildId}/${this.caseId}/comments/${commentId}`).subscribe(() => {
      this.toastr.success("Comment deleted");
      this.reloadCase();
    }, () => {
      this.toastr.error("Failed to delete comment.");
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
        this.api.putSimpleData(`/modcases/${this.guildId}/${this.caseId}/comments/${comment.id}`, data).subscribe(() => {
          this.toastr.success("Comment updated.");
          this.reloadCase();
        }, () => {
          this.toastr.error("Failed to update comment.");
        });
      }
    });
    this.reloadCase();
  }

  postComment() {
    const data = {
      "message": this.newComment.trim()
    };

    this.api.postSimpleData(`/modcases/${this.guildId}/${this.caseId}/comments`, data).subscribe(() => {
      this.newComment = "";
      this.toastr.success("Comment posted.");
      this.reloadCase();
    }, () => {
      this.toastr.error("Failed to post comment.");
    });
  }

  lockCaseComments() {
    this.api.postSimpleData(`/modcases/${this.guildId}/${this.caseId}/lock`, null).subscribe(() => {
      this.toastr.success("Locked comments.");
      this.reloadCase();
    }, () => {
      this.toastr.error("Failed to lock comments.");
    });
  }

  unlockCaseComments() {
    this.api.deleteData(`/modcases/${this.guildId}/${this.caseId}/lock`).subscribe(() => {
      this.toastr.success("Unlocked comments.");
      this.reloadCase();
    }, () => {
      this.toastr.error("Failed to unlock comments.");
    });
  }

  safe_tags_replace(str: any) {
    return str.replace(/[&<>]/g, replaceTag);
  }

  renderDescription(str: string, guildId: string): string {
    return this.safe_tags_replace(str).replace(/#(\d+)/g, function(match: any, id: any) {
      return `<a href="/guilds/${guildId}/cases/${id}">#${id}</a>`
    });
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
