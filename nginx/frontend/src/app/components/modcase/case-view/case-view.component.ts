import { HttpParams } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { AppUser } from 'src/app/models/AppUser';
import { CaseView } from 'src/app/models/CaseView';
import { DiscordUser } from 'src/app/models/DiscordUser';
import { FileInfo } from 'src/app/models/FileInfo';
import { Guild } from 'src/app/models/Guild';
import { ModCase } from 'src/app/models/ModCase';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';
import Swal from 'sweetalert2'

@Component({
  selector: 'app-case-view',
  templateUrl: './case-view.component.html',
  styleUrls: ['./case-view.component.scss']
})
export class CaseViewComponent implements OnInit {

  @Input() newComment!: string;

  previewFiles: string[] = ['jpg', 'png', 'jpeg', 'gif', 'ico', 'tif', 'tiff'];
  guildId!: string | null;
  caseId!: string | null;
  currentUser!: Observable<AppUser>;
  caseView!: Promise<CaseView>;
  caseLoading: boolean = true
  guild!: Promise<Guild>;
  files!: Promise<FileInfo>;
  isModOrHigher: boolean = false;
  fileToUpload!: File | null;

  users: { [key: string]: Promise<DiscordUser> } = {};
  
  constructor(private route: ActivatedRoute, private auth: AuthService, private toastr: ToastrService, private api: ApiService, public router: Router) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid');
    this.caseId = this.route.snapshot.paramMap.get('caseid');
    
    this.guild = this.api.getSimpleData(`/discord/guilds/${this.guildId}`).toPromise();

    this.auth.getUserProfile().subscribe((data) => {
      this.isModOrHigher = data.modGuilds.find(x => x.id === this.guildId) !== undefined || data.adminGuilds.find(x => x.id === this.guildId) !== undefined || data.isAdmin;
    });
    this.currentUser = this.auth.getUserProfile();

    this.files = this.api.getSimpleData(`/guilds/${this.guildId}/modcases/${this.caseId}/files`).toPromise();
    this.caseView = this.api.getSimpleData(`/guilds/${this.guildId}/modcases/${this.caseId}/view`).toPromise();
    this.caseView.then(() => { this.caseLoading = false; });
  }

  redirectToApi() {
    window.location.href = `/api/v1/modcases/${this.guildId}/${this.caseId}`;
  }

  updateComment(commentId: number) {    
    this.toastr.warning("This feature is not yet available.");
  }

  deleteComment(commentId: number) {
    Swal.fire({
      title: 'Caution!',
      text: `Do you want to delete this comment? (Id: ${commentId})`,
      icon: 'warning',
      confirmButtonText: 'Delete',
      showCancelButton: true
    }).then((data) => {
      if(data.isConfirmed) {
        this.api.deleteData(`/modcases/${this.guildId}/${this.caseId}/comments/${commentId}`).subscribe((data) => {
          this.toastr.success('Comment deleted.');
          this.caseView = this.api.getSimpleData(`/guilds/${this.guildId}/modcases/${this.caseId}/view`).toPromise();
        }, (error) => {
          this.toastr.error('Cannot delete comment.', 'Something went wrong.');
        });
      }
    });
  }

  deleteFile(filename: string) {
    Swal.fire({
      title: 'Caution!',
      text: `Do you want to delete this file? (Filename: ${filename})`,
      icon: 'warning',
      confirmButtonText: 'Delete',
      showCancelButton: true
    }).then((data) => {
      if(data.isConfirmed) {
        this.api.deleteData(`/guilds/${this.guildId}/modcases/${this.caseId}/files/${filename}`).subscribe((data) => {
          this.toastr.success('File deleted.');
          this.files = this.api.getSimpleData(`/guilds/${this.guildId}/modcases/${this.caseId}/files`).toPromise();
        }, (error) => {
          this.toastr.error('Cannot delete file.', 'Something went wrong.');
        });
      }
    });
  }

  sendComment() {
    if (this.newComment) {
      let c = this.newComment;
      this.newComment = '';
      this.api.postSimpleData(`/modcases/${this.guildId}/${this.caseId}/comments`, { 'message': c }).subscribe((data) => {
        this.toastr.success('Comment posted.');
        this.caseView = this.api.getSimpleData(`/guilds/${this.guildId}/modcases/${this.caseId}/view`).toPromise();
      }, (error) => {
        this.toastr.error('Cannot post comment.', 'Something went wrong.');
      });
    }
  }

  uploadFile() {
    if (this.fileToUpload) {
      this.api.postFile(`/guilds/${this.guildId}/modcases/${this.caseId}/files/`, this.fileToUpload).subscribe((data) => {
        this.toastr.success('File uploaded.');
        this.files = this.api.getSimpleData(`/guilds/${this.guildId}/modcases/${this.caseId}/files`).toPromise();
      }, (error) => {
        this.toastr.error('Cannot upload file.', 'Something went wrong.');
      });
    }
  }

  deleteCase() {
    Swal.fire({
      title: 'Caution!',
      text: `Do you want to delete this case?`,
      icon: 'warning',
      confirmButtonText: 'Delete',
      input: 'checkbox',
      inputValue: 0,
      inputPlaceholder: 'Send public notification',
      showCancelButton: true
    }).then((data) => {
      if(data.isConfirmed) {
        let params = new HttpParams()
          .set('sendnotification', data?.value ? 'true' : 'false');
        this.api.deleteData(`/modcases/${this.guildId}/${this.caseId}`, params).subscribe((data) => {
          this.toastr.success('Case deleted.');
          this.router.navigate(['guilds', this.guildId])
        }, (error) => {
          this.toastr.error('Cannot delete case.', 'Something went wrong.');
        });
      }
    });
  }

  handleFileInput(event: any) {
    this.fileToUpload = event.target.files.item(0);
  }
}
