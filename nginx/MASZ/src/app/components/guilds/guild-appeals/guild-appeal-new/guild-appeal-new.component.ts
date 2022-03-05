import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { AppealStructureMode } from 'src/app/models/AppealStructureMode';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { IAppealStructure } from 'src/app/models/IAppealStructure';
import { IAppealView } from 'src/app/models/IAppealView';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';
import { EnumManagerService } from 'src/app/services/enum-manager.service';

@Component({
  selector: 'app-guild-appeal-new',
  templateUrl: './guild-appeal-new.component.html',
  styleUrls: ['./guild-appeal-new.component.css']
})
export class GuildAppealNewComponent implements OnInit {

  AppealStructureMode = AppealStructureMode;
  public guildId!: string;
  public newQuestionFormGroup!: FormGroup;

  public answers: { questionId: number, answer: string }[] = [];
  public questions: ContentLoading<IAppealStructure[]> = { content: [], loading: true };

  constructor(private route: ActivatedRoute, private auth: AuthService, private toastr: ToastrService, private api: ApiService, public router: Router, private _formBuilder: FormBuilder, private dialog: MatDialog, private enumManager: EnumManagerService, private translator: TranslateService) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid') as string;

    this.newQuestionFormGroup = this._formBuilder.group({
      question: ['', [ Validators.required, Validators.maxLength(4096) ]]
    });

    this.reload();
  }

  reload() {
    this.api.getSimpleData(`/guilds/${this.guildId}/appealstructures`).subscribe((data: IAppealStructure[]) => {
      this.questions.content = data;
      this.questions.loading = false;
      this.answers = data.map(x => { return { questionId: x.id, answer: "" }; });
    }, error => {
      console.error(error);
      this.toastr.error("oh no");
    });
  }

  saveAnswer(event: {id: number, answer: string}) {
    const index = this.answers.findIndex(a => a.questionId === event.id);
    if (index === -1) {
      this.answers.push({ questionId: event.id, answer: event.answer });
    } else {
      this.answers[index].answer = event.answer;
    }
  }

  createAppeal() {
    const data = {
      answers: this.answers,
    }
    this.api.postSimpleData(`/guilds/${this.guildId}/appeal`, data).subscribe((data: IAppealView) => {
      this.toastr.success("Appeal created");
      this.router.navigate(['/guilds', this.guildId, 'appeals', data.id]);
    }, error => {
      console.error(error);
      this.toastr.error("oh no");
    });
  }
}
