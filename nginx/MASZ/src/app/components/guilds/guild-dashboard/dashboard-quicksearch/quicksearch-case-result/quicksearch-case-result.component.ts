import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-quicksearch-case-result',
  templateUrl: './quicksearch-case-result.component.html',
  styleUrls: ['./quicksearch-case-result.component.css']
})
export class QuicksearchCaseResultComponent implements OnInit {

  @Input() caseEntry!: any;
  constructor() { }

  ngOnInit(): void {
  }

}
