import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CaseView } from 'src/app/models/CaseView';
import { ModCaseTable } from 'src/app/models/ModCaseTable';

@Component({
  selector: 'app-case-card',
  templateUrl: './case-card.component.html',
  styleUrls: ['./case-card.component.scss']
})
export class CaseCardComponent implements OnInit {

  @Input() entry: ModCaseTable;

  constructor(public router: Router) { }

  ngOnInit(): void {
  }

}
