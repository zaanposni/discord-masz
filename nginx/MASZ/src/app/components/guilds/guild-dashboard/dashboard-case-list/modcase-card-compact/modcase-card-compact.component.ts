import { Component, Input, OnInit } from '@angular/core';
import { APIEnumTypes } from 'src/app/models/APIEmumTypes';
import { APIEnum } from 'src/app/models/APIEnum';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { convertModcaseToPunishmentString } from 'src/app/models/ModCase';
import { ModCaseTable } from 'src/app/models/ModCaseTable';
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

  public punishment: string = "Unknown";
  public punishments: ContentLoading<APIEnum[]> = { loading: true, content: [] };

  constructor(private enumManager: EnumManagerService) { }

  ngOnInit(): void {
    this.enumManager.getEnum(APIEnumTypes.PUNISHMENT).subscribe((data) => {
      this.punishments.loading = false;
      this.punishments.content = data;
      this.punishment = convertModcaseToPunishmentString(this.entry?.modCase, this.punishments.content);
    }, () => {
      this.punishments.loading = false;
    });
  }

}
