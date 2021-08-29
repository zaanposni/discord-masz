import { Component, Input, OnInit } from '@angular/core';
import { APIEnum } from 'src/app/models/APIEnum';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { convertModcaseToPunishmentString } from 'src/app/models/ModCase';
import { ModCaseTable } from 'src/app/models/ModCaseTable';
import { EnumManagerService } from 'src/app/services/enum-manager.service';

@Component({
  selector: 'app-modcase-card',
  templateUrl: './modcase-card.component.html',
  styleUrls: ['./modcase-card.component.css']
})
export class ModcaseCardComponent implements OnInit {
  public punishment: string = "Unknown";
  public punishments: ContentLoading<APIEnum[]> = { loading: true, content: [] };
  @Input() entry!: ModCaseTable;
  constructor(private enumManager: EnumManagerService) { }

  ngOnInit(): void {
    this.reloadPunishmentEnum();
  }

  private reloadPunishmentEnum() {
    this.punishments = { loading: true, content: [] };
    this.enumManager.getEnum('punishment').subscribe((data) => {
      this.punishments.loading = false;
      this.punishments.content = data;
      this.punishment = convertModcaseToPunishmentString(this.entry?.modCase, this.punishments.content);
    }, () => {
      this.punishments.loading = false;
    });
  }

}
