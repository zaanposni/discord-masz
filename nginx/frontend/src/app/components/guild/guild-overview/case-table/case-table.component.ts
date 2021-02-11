import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ModCase } from 'src/app/models/ModCase';
import { ModCaseTable } from 'src/app/models/ModCaseTable';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-case-table',
  templateUrl: './case-table.component.html',
  styleUrls: ['./case-table.component.scss']
})
export class CaseTableComponent implements OnInit {
  
  apiUrl: string = 'modcasetable'
  currentPage: number = 0;

  showTable: ModCaseTable[] = new Array<ModCaseTable>();
  casesTable: ModCaseTable[] = new Array<ModCaseTable>();
  @Input() uniqueIdentifier: string;
  @Input() isModOrHigher: boolean = false;
  @Input() guildId: string;
  @Input() punishmentTable: boolean = false;
  loading: boolean = true;

  @Input() search: string;
  timeout: any = null;

  excludePermaPunishments: boolean = false;
  excludeAutoModeration: boolean = false;

  constructor(public router: Router, private api: ApiService) { }

  ngOnInit(): void {
    if (this.punishmentTable) {
      this.apiUrl = "punishmenttable";
    }
    this.loadFirstCases();
  }

  searchChanged(event: any) {
    clearTimeout(this.timeout);
    var $this = this;
    this.timeout = setTimeout(function () {
      if (event.keyCode != 13) {
        $this.executeListing(event);
      }
    }, 500);
  }

  private executeListing(value: string) {
    if (value.trim()) {
      this.loading = true;
      this.api.getSimpleData(`/guilds/${this.guildId}/${this.apiUrl}?search=${encodeURIComponent(value)}`).toPromise().then((data) => {
        this.loading = false;
        this.casesTable = data;
        this.applyCurrentFilters();
      });
    } else {
      this.loadFirstCases();
    }
  }

  loadFirstCases() {
    this.loading = true;
    this.currentPage = 0;
    this.api.getSimpleData(`/guilds/${this.guildId}/${this.apiUrl}`).toPromise().then((data) => {
      this.loading = false;
      this.casesTable = data;
      this.applyCurrentFilters();
    });
  }

  loadNextPage() {
    this.loading = true;
    this.currentPage++;
    this.api.getSimpleData(`/guilds/${this.guildId}/${this.apiUrl}?startPage=${this.currentPage}`).toPromise().then((data) => {
      this.loading = false;
      data.forEach((element: ModCaseTable ) => {
        this.casesTable.push(element);
      });
      this.applyCurrentFilters();
    });
  }

  applyCurrentFilters() {
    let temp = this.casesTable;
    if (this.excludePermaPunishments) {
      temp = temp.filter(x => x.modCase.punishedUntil != null || x.modCase.punishmentType === 0 || x.modCase.punishmentType === 2);
    }
    if (this.excludeAutoModeration) {
      temp = temp.filter(x => x.modCase.creationType !== 1);
    }
    this.showTable = temp;
  }
}
