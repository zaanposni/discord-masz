import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationDialogComponent } from 'src/app/components/dialogs/confirmation-dialog/confirmation-dialog.component';
import { AppealStructureEditDialogComponent } from 'src/app/components/dialogs/appeal-structure-edit-dialog/appeal-structure-edit-dialog.component';
import { IAppealAnswer } from 'src/app/models/IAppealAnswer';
import { IAppealStructure } from 'src/app/models/IAppealStructure';
import { ApiService } from 'src/app/services/api.service';
import { AppealStructureMode } from 'src/app/models/AppealStructureMode';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-guild-appeal-question',
  templateUrl: './guild-appeal-question.component.html',
  styleUrls: ['./guild-appeal-question.component.css']
})
export class GuildAppealQuestionComponent implements OnInit {

  AppealStructureMode = AppealStructureMode;
  @Input() mode: AppealStructureMode = AppealStructureMode.VIEW;
  @Input() question?: IAppealStructure;
  @Input() answer?: IAppealAnswer;

  @Output() answerEvent = new EventEmitter<{id: number, answer: string}>();
  @Output() deleteEvent = new EventEmitter<number>();

  maxLength10240 = { length: 10240 };
  public answerForm!: FormGroup;

  constructor(private dialog: MatDialog, private toastr: ToastrService, private api: ApiService, private _formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.answerForm = this._formBuilder.group({
      answer: ['', [ Validators.maxLength(10240) ]]
    });

    this.answerForm.valueChanges.subscribe(() => {
      this.answerEvent.emit({ id: this.question!.id, answer: this.answerForm.value.answer });
    });
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

  get newAnswer() { return this.answerForm.get('answer'); }
}
