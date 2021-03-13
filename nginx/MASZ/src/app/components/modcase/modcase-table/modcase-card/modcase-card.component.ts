import { Component, Input, OnInit } from '@angular/core';
import { ModCaseTable } from 'src/app/models/ModCaseTable';

@Component({
  selector: 'app-modcase-card',
  templateUrl: './modcase-card.component.html',
  styleUrls: ['./modcase-card.component.css']
})
export class ModcaseCardComponent implements OnInit {

  @Input() entry!: ModCaseTable;
  constructor() { }

  ngOnInit(): void {
  }

}
