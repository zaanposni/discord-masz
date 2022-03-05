import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationDialogComponent } from 'src/app/components/dialogs/confirmation-dialog/confirmation-dialog.component';
import { AppealStructureEditDialogComponent } from 'src/app/components/dialogs/appeal-structure-edit-dialog/appeal-structure-edit-dialog.component';
import { IAppealAnswer } from 'src/app/models/IAppealAnswer';
import { IAppealStructure } from 'src/app/models/IAppealStructure';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-guild-appeal-question',
  templateUrl: './guild-appeal-question.component.html',
  styleUrls: ['./guild-appeal-question.component.css']
})
export class GuildAppealQuestionComponent implements OnInit {

  @Input() showActionButtons: boolean = false;
  @Input() question?: IAppealStructure;
  @Input() answer?: IAppealAnswer;

  @Output() deleteEvent = new EventEmitter<number>();

  constructor(private dialog: MatDialog, private toastr: ToastrService, private api: ApiService) { }

  ngOnInit(): void {
  }

  edit() {
    let questionDto: IAppealStructure = {
      id: this.question!.id,
      guildId: this.question!.guildId,
      question: this.question!.question,
      sortOrder: this.question!.sortOrder
    };
    const editDialogRef = this.dialog.open(AppealStructureEditDialogComponent, {
      data: questionDto,
      minWidth: '400px'
    });
    editDialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.api.putSimpleData(`/guilds/${this.question?.guildId}/appealstructures/${this.question?.id}`, questionDto).subscribe(() => {
          this.toastr.success("updated");
          this.question!.question = questionDto.question.trim();
        }, error => {
          console.error(error);
          this.toastr.error("oh no");
        });
      }
    });
  }

  delete() {
    const confirmDialogRef = this.dialog.open(ConfirmationDialogComponent);
    confirmDialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.api.deleteData(`/guilds/${this.question?.guildId}/appealstructures/${this.question?.id}`).subscribe(() => {
          this.deleteEvent.emit(this.question?.id);
          this.toastr.success("deleted");
        }, error => {
          console.error(error);
          this.toastr.error("failed to delete");
        })
      }
    });
  }
}
