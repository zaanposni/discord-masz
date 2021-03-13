import { Component, Input, OnInit } from '@angular/core';
import { ModCaseTable } from 'src/app/models/ModCaseTable';

@Component({
  selector: 'app-case-card-compact',
  templateUrl: './case-card-compact.component.html',
  styleUrls: ['./case-card-compact.component.scss']
})
export class CaseCardCompactComponent implements OnInit {

  @Input() entry: ModCaseTable;
  @Input() showExpiring: boolean = true;
  @Input() showCreated: boolean = false;

  constructor() { }

  ngOnInit(): void {
  }

}
