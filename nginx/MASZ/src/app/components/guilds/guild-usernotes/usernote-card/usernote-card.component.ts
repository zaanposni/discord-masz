import { EventEmitter } from '@angular/core';
import { Component, Input, OnInit, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
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
  constructor(private dialog: MatDialog, private api: ApiService, private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  deleteNote() {
    const confirmDialogRef = this.dialog.open(ConfirmationDialogComponent);
    confirmDialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.api.deleteData(`/guilds/${this.userNote.userNote.guildId}/usernote/${this.userNote.userNote.id}`).subscribe((data) => {
          this.deleteEvent.emit(this.userNote.userNote.id);
          this.toastr.success('Usernote deleted.');
        }, () => {
          this.toastr.error('Failed to delete usernote.');
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
        this.api.putSimpleData(`/guilds/${this.userNote.userNote.guildId}/usernote`, userNoteDto).subscribe((data) => {
          this.toastr.success('Usernote updated.');
          this.userNote.userNote.description = userNoteDto.description.trim();
          this.updateEvent.emit(0);
        }, () => {
          this.toastr.error('Failed to update usernote.');
        });
      }
    });
  }
}
