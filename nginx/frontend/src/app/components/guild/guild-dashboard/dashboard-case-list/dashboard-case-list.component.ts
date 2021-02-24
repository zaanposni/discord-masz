import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CaseView } from 'src/app/models/CaseView';
import { ModCase } from 'src/app/models/ModCase';
import { ModCaseTable } from 'src/app/models/ModCaseTable';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-dashboard-case-list',
  templateUrl: './dashboard-case-list.component.html',
  styleUrls: ['./dashboard-case-list.component.scss']
})
export class DashboardCaseListComponent implements OnInit {

  @Input() title: string;
  @Input() resource: string;
  private guildId: string;
  public cases: ModCaseTable[] = [];
  public loading: boolean = true;

  constructor(private route: ActivatedRoute, private api: ApiService, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid');
    this.reload();
  }

  private reload() {
    this.loading = true;
    this.cases = [];
    this.api.getSimpleData(`/guilds/${this.guildId}/${this.resource}`).subscribe((data: ModCaseTable[]) => {
      this.cases = data.slice(0, 5);
      this.loading = false;
    }, () => { this.loading = false; });
  }
}
