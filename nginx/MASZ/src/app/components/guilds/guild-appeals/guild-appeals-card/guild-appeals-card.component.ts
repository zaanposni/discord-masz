import { Component, Input, OnInit } from '@angular/core';
import { APIEnumTypes } from 'src/app/models/APIEmumTypes';
import { APIEnum } from 'src/app/models/APIEnum';
import { AppealStatus } from 'src/app/models/AppealStatus';
import { IAppealView } from 'src/app/models/IAppealView';
import { ApiService } from 'src/app/services/api.service';
import { EnumManagerService } from 'src/app/services/enum-manager.service';

@Component({
  selector: 'app-guild-appeals-card',
  templateUrl: './guild-appeals-card.component.html',
  styleUrls: ['./guild-appeals-card.component.css']
})
export class GuildAppealsCardComponent implements OnInit {

  AppealStatus = AppealStatus;
  @Input() entry!: IAppealView;
  public status: string = "Unknown";
  questionsAnswered: { count: number } = { count: 0 };

  constructor(private enumManager: EnumManagerService) { }

  ngOnInit(): void {
    this.enumManager.getEnum(APIEnumTypes.APPEALSTATUS).subscribe((data: APIEnum[]) => {
      this.status = data.find(x => x.key === this.entry.status)?.value ?? "Unknown";
    });

    this.questionsAnswered = { count: this.entry.answers.filter(x => x.answer?.trim()).length };
  }
}
