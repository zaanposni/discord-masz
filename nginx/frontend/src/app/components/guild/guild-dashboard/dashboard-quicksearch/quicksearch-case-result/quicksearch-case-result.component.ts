import { Component, Input, OnInit } from '@angular/core';
import { ModCaseTable } from 'src/app/models/ModCaseTable';

@Component({
  selector: 'app-quicksearch-case-result',
  templateUrl: './quicksearch-case-result.component.html',
  styleUrls: ['./quicksearch-case-result.component.scss']
})
export class QuicksearchCaseResultComponent implements OnInit {

  @Input() caseEntry!: any;

  constructor() { }

  ngOnInit(): void {
  }

}
