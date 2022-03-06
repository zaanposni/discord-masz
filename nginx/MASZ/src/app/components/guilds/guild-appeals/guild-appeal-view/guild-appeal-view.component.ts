import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { dateInputsHaveChanged } from '@angular/material/datepicker/datepicker-input-base';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Moment } from 'moment';
import { ToastrService } from 'ngx-toastr';
import { ReplaySubject } from 'rxjs';
import { APIEnumTypes } from 'src/app/models/APIEmumTypes';
import { APIEnum } from 'src/app/models/APIEnum';
import { AppealStatus } from 'src/app/models/AppealStatus';
import { AppealStructureMode } from 'src/app/models/AppealStructureMode';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { IAppealAnswer } from 'src/app/models/IAppealAnswer';
import { IAppealView } from 'src/app/models/IAppealView';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';
import { EnumManagerService } from 'src/app/services/enum-manager.service';

@Component({
  selector: 'app-guild-appeal-view',
  templateUrl: './guild-appeal-view.component.html',
  styleUrls: ['./guild-appeal-view.component.css']
})
export class GuildAppealViewComponent implements OnInit {

  AppealStatus = AppealStatus;
  AppealStructureMode = AppealStructureMode;
  public guildId!: string;
  public appealId!: string;
  public appeal: ContentLoading<IAppealView> = { content: undefined, loading: true };
  public status: string = "Unknown";
  public statusEnum: APIEnum[] = [];
  public editForm!: FormGroup;
  public isModOrHigher: boolean = false;
  public moderatorCommentInitRows: number = 2;
  public scheduledForChangedForPicker: ReplaySubject<Date> = new ReplaySubject<Date>(1);

  constructor(private _formBuilder: FormBuilder, public router: Router, private api: ApiService, private auth: AuthService, private route: ActivatedRoute, private toastr: ToastrService, private translator: TranslateService, private enumManager: EnumManagerService) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid') as string;
    this.appealId = this.route.snapshot.paramMap.get('appealid') as string;

    this.editForm = this._formBuilder.group({
      moderatorComment: ['', [ Validators.required ]],
      status: ['', [ Validators.required ]],
      neverAllowed: [''],
      allowedAt: [''],
    });

    this.auth.isModInGuild(this.guildId).subscribe(isMod => {
      this.isModOrHigher = isMod;
    });

    this.enumManager.getEnum(APIEnumTypes.APPEALSTATUS).subscribe(data => {
      this.statusEnum = data;
      this.status = this.statusEnum.find(x => x.key === this.appeal.content?.status)?.value ?? "Unknown";
    });
    this.reload();
  }

  reload() {
    this.appeal.loading = true;

    this.api.getSimpleData(`/guilds/${this.guildId}/appeal/${this.appealId}`).subscribe((data: IAppealView) => {
      this.appeal.loading = false;
      this.applyNewValues(data);
    }, error => {
      console.error(error);
      this.appeal.loading = false;
      this.toastr.error("oh no");
    });
  }

  applyNewValues(data: IAppealView) {
    this.appeal.content = data;

    this.status = this.statusEnum.find(x => x.key === this.appeal.content?.status)?.value ?? "Unknown";

    this.editForm.setValue({
      moderatorComment: this.appeal.content?.moderatorComment ?? '',
      status: this.appeal.content?.status ?? AppealStatus.Pending,
      neverAllowed: this.appeal.content?.userCanCreateNewAppeals ? false : true,
      allowedAt: this.appeal.content?.userCanCreateNewAppeals
    });

    this.moderatorCommentInitRows = Math.min(this.appeal.content?.moderatorComment?.split('\n').length ?? 2, 20);

    if (this.appeal.content?.userCanCreateNewAppeals) {
      this.scheduledForChangedForPicker.next(this.appeal.content?.userCanCreateNewAppeals);
    }
  }

  getQuestionByAnswer(answer: IAppealAnswer) {
    return this.appeal.content?.structures.find(x => x.id === answer.questionId);
  }

  dateChanged(date: Moment) {
    this.editForm.get('allowedAt')!.setValue(date);
  }

  update() {
    const data = {
      status: this.editForm.value.status,
      moderatorComment: this.editForm.value.moderatorComment,
      userCanCreateNewAppeals: this.editForm.value.neverAllowed ? null : this.editForm.value.allowedAt.toISOString()
    }
    this.api.putSimpleData(`/guilds/${this.guildId}/appeal/${this.appealId}`, data).subscribe((res: IAppealView) => {
      res.latestCases = this.appeal.content!.latestCases;
      this.applyNewValues(res);
      this.toastr.success(this.translator.instant("AppealView.Updated"));
    }, error => {
      console.error(error);
      this.toastr.error(this.translator.instant("AppealView.FailedToUpdate"));
    });
  }

  get newStatus() { return this.editForm.get('status'); }
  get newComment() { return this.editForm.get('moderatorComment'); }
  get neverAllowed() { return this.editForm.get('neverAllowed'); }
  get allowedAt() { return this.editForm.get('allowedAt'); }
}
