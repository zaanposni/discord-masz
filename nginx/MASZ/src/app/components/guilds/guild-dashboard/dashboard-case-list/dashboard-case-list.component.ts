import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { IModCaseTable } from 'src/app/models/IModCaseTable';
import { IModCaseTableEntry } from 'src/app/models/IModCaseTableEntry';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-dashboard-case-list',
  templateUrl: './dashboard-case-list.component.html',
  styleUrls: ['./dashboard-case-list.component.css']
})
export class DashboardCaseListComponent implements OnInit {

  @Input() title!: string;
  @Input() resource!: string;
  private guildId!: string;
  public cases: IModCaseTableEntry[] = [];
  public loading: boolean = true;

  constructor(private route: ActivatedRoute, private api: ApiService) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid') as string;
    this.reload();
  }

  private reload() {
    this.loading = true;
    this.cases = [];
    this.api.postSimpleData(`/guilds/${this.guildId}/${this.resource}`, {}).subscribe((data: IModCaseTable) => {
      this.cases = data.cases.slice(0, 5);
      this.loading = false;
    }, error => {
      console.error(error);
      this.loading = false;
    });
  }

}
