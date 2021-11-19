import { Component, Input, OnInit } from '@angular/core';
import { APIEnumTypes } from 'src/app/models/APIEmumTypes';
import { APIEnum } from 'src/app/models/APIEnum';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { convertModcaseToPunishmentString } from 'src/app/models/ModCase';
import { ModCaseTable } from 'src/app/models/ModCaseTable';
import { PunishmentType } from 'src/app/models/PunishmentType';
import { EnumManagerService } from 'src/app/services/enum-manager.service';

@Component({
  selector: 'app-modcase-card-compact',
  templateUrl: './modcase-card-compact.component.html',
  styleUrls: ['./modcase-card-compact.component.css']
})
export class ModcaseCardCompactComponent implements OnInit {

  @Input() entry!: ModCaseTable;
  @Input() showExpiring: boolean = true;
  @Input() showCreated: boolean = false;

  public punishmentTooltip: string = "";
  public punishment: string = "Unknown";
  public punishments: ContentLoading<APIEnum[]> = { loading: true, content: [] };

  constructor(private enumManager: EnumManagerService) { }

  ngOnInit(): void {
    if (this.entry.modCase.punishmentType !== PunishmentType.None && ! this.entry.modCase.punishmentActive) {
      if (this.entry.modCase.punishedUntil === null) {
        this.punishmentTooltip = "Modcase deactivated.";
      } else if (new Date(this.entry.modCase.punishedUntil) > new Date()) {
        this.punishmentTooltip = "Modcase deactivated.";
      } else {
        this.punishmentTooltip = "Modcase expired.";
      }
    }
    this.enumManager.getEnum(APIEnumTypes.PUNISHMENT).subscribe((data) => {
      this.punishments.loading = false;
      this.punishments.content = data;
      this.punishment = convertModcaseToPunishmentString(this.entry?.modCase, this.punishments.content);
    }, () => {
      this.punishments.loading = false;
    });
  }

}
