import { HttpParams } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Toast, ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { AppUser } from 'src/app/models/AppUser';
import { CaseComment } from 'src/app/models/CaseComment';
import { DiscordUser } from 'src/app/models/DiscordUser';
import { FileInfo } from 'src/app/models/FileInfo';
import { Guild } from 'src/app/models/Guild';
import { ModCase } from 'src/app/models/ModCase';
import { ApiCacheService } from 'src/app/services/api-cache.service';
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
  modCase!: Promise<ModCase>;
  guild!: Promise<Guild>;
  files!: Promise<FileInfo>;
  isModOrHigher: boolean = false;
  fileToUpload!: File | null;

  users: { [key: string]: Promise<DiscordUser> } = {};
  
  constructor(private cache: ApiCacheService, private route: ActivatedRoute, private auth: AuthService, private toastr: ToastrService, private api: ApiService, private router: Router) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid');
    this.caseId = this.route.snapshot.paramMap.get('caseid');

    this.auth.getUserProfile().subscribe((data) => {
      this.isModOrHigher = data.modGuilds.find(x => x.id === this.guildId) !== undefined || data.adminGuilds.find(x => x.id === this.guildId) !== undefined || data.isAdmin;
    });
    this.currentUser = this.auth.getUserProfile();


    this.files = this.cache.getSimpleData(`/guilds/${this.guildId}/modcases/${this.caseId}/files`);
    this.modCase = this.cache.getSimpleData(`/modcases/${this.guildId}/${this.caseId}`);

    this.modCase.then((data) => {
      this.guild = this.cache.getSimpleData(`/discord/guilds/${data.guildId}`);

      this.users[data.userId] = this.cache.getSimpleData(`/discord/users/${data.userId}`);
      this.users[data.modId] = this.cache.getSimpleData(`/discord/users/${data.modId}`);
      if (data.modId !== data.lastEditedByModId) {
        this.users[data.lastEditedByModId] = this.cache.getSimpleData(`/discord/users/${data.lastEditedByModId}`);
      }

      data.comments.forEach(element => {
        if ( !(element.userId in this.users) ) {
          this.users[element.userId] = this.cache.getSimpleData(`/discord/users/${element.userId}`);
        }
      });
    }, (error) => { });
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
          this.modCase = this.api.getSimpleData(`/modcases/${this.guildId}/${this.caseId}`).toPromise();
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
        this.modCase = this.api.getSimpleData(`/modcases/${this.guildId}/${this.caseId}`).toPromise();
      }, (error) => {
        console.log(error.message);
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
        console.log(error.message);
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

  handleFileInput(files: FileList) {
    this.fileToUpload = files.item(0);
  }
}
