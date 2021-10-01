import { EventEmitter } from '@angular/core';
import { Component, Input, OnInit, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationDialogComponent } from 'src/app/components/dialogs/confirmation-dialog/confirmation-dialog.component';
import { UsernoteEditDialogComponent } from 'src/app/components/dialogs/usernote-edit-dialog/usernote-edit-dialog.component';
import { UserNoteDto } from 'src/app/models/UserNoteDto';
import { UserNoteView } from 'src/app/models/UserNoteView';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-usernote-card',
  templateUrl: './usernote-card.component.html',
  styleUrls: ['./usernote-card.component.css']
})
export class UsernoteCardComponent implements OnInit {

  @Output() updateEvent = new EventEmitter<number>();
  @Output() deleteEvent = new EventEmitter<number>();
  @Input() userNote!: UserNoteView;
  @Input() showDeleteButton: boolean = true;
  constructor(private dialog: MatDialog, private api: ApiService, private toastr: ToastrService, private translator: TranslateService) { }

  ngOnInit(): void {
  }

  deleteNote() {
    const confirmDialogRef = this.dialog.open(ConfirmationDialogComponent);
    confirmDialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.api.deleteData(`/guilds/${this.userNote.userNote.guildId}/usernote/${this.userNote.userNote.userId}`).subscribe(() => {
          this.deleteEvent.emit(this.userNote.userNote.id);
          this.toastr.success(this.translator.instant('UsernoteCard.Deleted'));
        }, error => {
          console.error(error);
          this.toastr.error(this.translator.instant('UsernoteCard.FailedToDelete'));
        })
      }
    });
  }

  editNote() {
    let userNoteDto: UserNoteDto = {
      userid: this.userNote.userNote.userId,
      description: this.userNote.userNote.description
    };
    const editDialogRef = this.dialog.open(UsernoteEditDialogComponent, {
      data: userNoteDto,
      minWidth: '400px'
    });
    editDialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.api.putSimpleData(`/guilds/${this.userNote.userNote.guildId}/usernote`, userNoteDto).subscribe(() => {
          this.toastr.success(this.translator.instant('UsernoteCard.Updated'));
          this.userNote.userNote.description = userNoteDto.description.trim();
          this.userNote.userNote.updatedAt = new Date();
          this.updateEvent.emit(0);
        }, error => {
          console.error(error);
          this.toastr.error(this.translator.instant('UsernoteCard.FailedToUpdate'));
        });
      }
    });
  }
}
