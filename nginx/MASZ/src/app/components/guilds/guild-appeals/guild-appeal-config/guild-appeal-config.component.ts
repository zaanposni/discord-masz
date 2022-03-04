import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { IAppealStructure } from 'src/app/models/IAppealStructure';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';
import { EnumManagerService } from 'src/app/services/enum-manager.service';
import {CdkDragDrop, moveItemInArray} from '@angular/cdk/drag-drop';

@Component({
  selector: 'app-guild-appeal-config',
  templateUrl: './guild-appeal-config.component.html',
  styleUrls: ['./guild-appeal-config.component.css']
})
export class GuildAppealConfigComponent implements OnInit {

  public guildId!: string;
  public newQuestionFormGroup!: FormGroup;
  maxLength4096 = { length: 4096 };

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
    });
  }

  saveNewQuestion() {
    const data = {
      question: this.newQuestionFormGroup.value.question,
      sortOrder: Math.max(...this.questions.content ? this.questions.content.map(x => x.sortOrder) : [0], 0) + 1,
    };

    this.api.postSimpleData(`/guilds/${this.guildId}/appealstructures`, data).subscribe((data: IAppealStructure) => {
      if (this.questions.content) {
        this.questions.content.push(data);
      }
      this.newQuestionFormGroup.reset();
      Object.keys(this.newQuestionFormGroup.controls).forEach(key =>{
        this.newQuestionFormGroup.controls[key].setErrors(null)
      });
    }, error => {
      console.error(error);
      this.toastr.error("oh no");
    });
  }

  drop(event: CdkDragDrop<string[]>) {
    console.log(event);
    moveItemInArray(this.questions.content as [], event.previousIndex, event.currentIndex);

    const data = this.questions.content?.map((element, index) => {
      return {
        id: element.id,
        sortOrder: index
      };
    });
    console.log(data);

    this.api.putSimpleData(`/guilds/${this.guildId}/appealstructures/reorder`, data).subscribe(() => {
      this.toastr.success("Reordered");
    }, error => {
      console.error(error);
      this.toastr.error("oh no");
      moveItemInArray(this.questions.content as [], event.currentIndex, event.previousIndex);
    });
  }

  get newQuestion() { return this.newQuestionFormGroup.get('question'); }
}
