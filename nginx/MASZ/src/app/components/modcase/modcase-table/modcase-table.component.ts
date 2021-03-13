import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { ModCaseTable } from 'src/app/models/ModCaseTable';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-modcase-table',
  templateUrl: './modcase-table.component.html',
  styleUrls: ['./modcase-table.component.css']
})
export class ModcaseTableComponent implements OnInit {

  apiUrl: string = 'modcasetable'
  currentPage: number = 0;

  showTable: ModCaseTable[] = [];
  casesTable: ModCaseTable[] = [];
  isModOrHigher!: Observable<boolean>;
  guildId!: string;
  @Input() uniqueIdentifier: string = "casetable";
  @Input() punishmentTable: boolean = false;
  loading: boolean = true;

  @Input() search!: string;
  timeout: any = null;

  excludePermaPunishments: boolean = false;
  excludeAutoModeration: boolean = false;
  onlyShowActivePunishments: boolean = false;

  constructor(public router: Router, private api: ApiService, private auth: AuthService, private route: ActivatedRoute, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid') as string;
    if (this.punishmentTable) {
      this.apiUrl = "punishmenttable";
    }
    this.isModOrHigher = this.auth.isModInGuild(this.guildId);
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
      this.api.getSimpleData(`/guilds/${this.guildId}/${this.apiUrl}?search=${encodeURIComponent(value)}`).subscribe((data) => {
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
    this.api.getSimpleData(`/guilds/${this.guildId}/${this.apiUrl}`).subscribe((data) => {
      this.loading = false;
      this.casesTable = data;
      this.applyCurrentFilters();
    }, () => {
      this.toastr.error("Failed to load cases.");
    });
  }

  loadNextPage() {
    this.loading = true;
    this.currentPage++;
    this.api.getSimpleData(`/guilds/${this.guildId}/${this.apiUrl}?startPage=${this.currentPage}`).subscribe((data) => {
      this.loading = false;
      data.forEach((element: ModCaseTable ) => {
        this.casesTable.push(element);
      });
      this.applyCurrentFilters();
    }, () => {
      this.toastr.error("Failed to load cases.");
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
    if (this.onlyShowActivePunishments) {
      temp = temp.filter(x => x.modCase.punishmentActive);
    }
    this.showTable = temp;
  }

}
