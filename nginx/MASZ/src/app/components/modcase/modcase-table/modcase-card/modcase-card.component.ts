import { Component, Input, OnInit } from '@angular/core';
import { convertModcaseToPunishmentString } from 'src/app/models/ModCase';
import { ModCaseTable } from 'src/app/models/ModCaseTable';

@Component({
  selector: 'app-modcase-card',
  templateUrl: './modcase-card.component.html',
  styleUrls: ['./modcase-card.component.css']
})
export class ModcaseCardComponent implements OnInit {
  public convertModcaseToPunishmentString = convertModcaseToPunishmentString;
  @Input() entry!: ModCaseTable;
  constructor() { }

  ngOnInit(): void {
  }

}
