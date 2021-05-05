import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationDialogComponent } from 'src/app/components/dialogs/confirmation-dialog/confirmation-dialog.component';
import { UsermapEditDialogComponent } from 'src/app/components/dialogs/usermap-edit-dialog/usermap-edit-dialog.component';
import { UserMappingView } from 'src/app/models/UserMappingView';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-usermap-card',
  templateUrl: './usermap-card.component.html',
  styleUrls: ['./usermap-card.component.css']
})
export class UsermapCardComponent implements OnInit {

  @Output() updateEvent = new EventEmitter<number>();
  @Output() deleteEvent = new EventEmitter<number>();
  @Input() userMap!: UserMappingView;
  @Input() showDeleteButton: boolean = true;
  constructor(private dialog: MatDialog, private api: ApiService, private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  deleteMap() {
    const confirmDialogRef = this.dialog.open(ConfirmationDialogComponent);
    confirmDialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.api.deleteData(`/guilds/${this.userMap.userMapping.guildId}/usermap/${this.userMap.userMapping.id}`).subscribe((data) => {
          this.deleteEvent.emit(this.userMap.userMapping.id);
          this.toastr.success('Usermap deleted.');
        }, () => {
          this.toastr.error('Failed to delete usermap.');
        })
      }
    });
  }

  editMap() {
    let userMapDto: any = {
      reason: this.userMap.userMapping.reason
    };
    const editDialogRef = this.dialog.open(UsermapEditDialogComponent, {
      data: userMapDto,
      minWidth: '400px'
    });
    editDialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.api.putSimpleData(`/guilds/${this.userMap.userMapping.guildId}/usermap/${this.userMap.userMapping.id}`, userMapDto).subscribe((data) => {
          this.toastr.success('Usermap updated.');
          this.userMap.userMapping.reason = userMapDto.reason.trim();
          this.updateEvent.emit(0);
        }, () => {
          this.toastr.error('Failed to update usermap.');
        });
      }
    });
  }

}
