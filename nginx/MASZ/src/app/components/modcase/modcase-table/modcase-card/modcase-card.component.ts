import { Component, Input, OnInit } from '@angular/core';
import { APIEnumTypes } from 'src/app/models/APIEmumTypes';
import { APIEnum } from 'src/app/models/APIEnum';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { convertModcaseToPunishmentString } from 'src/app/models/ModCase';
import { IModCaseTableEntry } from 'src/app/models/IModCaseTableEntry';
import { PunishmentType } from 'src/app/models/PunishmentType';
import { EnumManagerService } from 'src/app/services/enum-manager.service';
import * as moment from 'moment';

@Component({
  selector: 'app-modcase-card',
  templateUrl: './modcase-card.component.html',
  styleUrls: ['./modcase-card.component.css']
})
export class ModcaseCardComponent implements OnInit {
  public punishmentTooltip: string = "";
  public punishment: string = "Unknown";
  public punishments: ContentLoading<APIEnum[]> = { loading: true, content: [] };
  @Input() entry!: IModCaseTableEntry;
  constructor(private enumManager: EnumManagerService) { }

  ngOnInit(): void {
    if (this.entry.modCase.punishmentType !== PunishmentType.None && ! this.entry.modCase.punishmentActive) {
      if (this.entry.modCase.punishedUntil === null) {
        this.punishmentTooltip = "Modcase deactivated.";
      } else if (moment(this.entry.modCase.punishedUntil).utc(true).isAfter(moment())) {
        this.punishmentTooltip = "Modcase deactivated.";
      } else {
        this.punishmentTooltip = "Modcase expired.";
      }
    }
    this.reloadPunishmentEnum();
  }

  private reloadPunishmentEnum() {
    this.punishments = { loading: true, content: [] };
    this.enumManager.getEnum(APIEnumTypes.PUNISHMENT).subscribe((data) => {
      this.punishments.loading = false;
      this.punishments.content = data;
      this.punishment = convertModcaseToPunishmentString(this.entry?.modCase, this.punishments.content);
    }, () => {
      this.punishments.loading = false;
    });
  }

}
