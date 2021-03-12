import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-dashboard-case-list',
  templateUrl: './dashboard-case-list.component.html',
  styleUrls: ['./dashboard-case-list.component.css']
})
export class DashboardCaseListComponent implements OnInit {

  @Input() title!: string;
  @Input() resource!: string;
  constructor() { }

  ngOnInit(): void {
  }

}
