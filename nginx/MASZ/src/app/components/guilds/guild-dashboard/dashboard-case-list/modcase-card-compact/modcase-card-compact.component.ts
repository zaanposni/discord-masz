import { Component, Input, OnInit } from '@angular/core';
import { ModCaseTable } from 'src/app/models/ModCaseTable';

@Component({
  selector: 'app-modcase-card-compact',
  templateUrl: './modcase-card-compact.component.html',
  styleUrls: ['./modcase-card-compact.component.css']
})
export class ModcaseCardCompactComponent implements OnInit {

  @Input() entry!: ModCaseTable;
  @Input() showExpiring: boolean = true;
  @Input() showCreated: boolean = false;

  constructor() { }

  ngOnInit(): void {
  }

}
