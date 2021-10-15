import { HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { QuickSearchResult } from 'src/app/models/QuickSearchResult';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-dashboard-quicksearch',
  templateUrl: './dashboard-quicksearch.component.html',
  styleUrls: ['./dashboard-quicksearch.component.css']
})
export class DashboardQuicksearchComponent implements OnInit {

  public guildId!: string;
  public showOverlay: boolean = false;
  public search!: string;
  public searchResults?: QuickSearchResult;
  public loading: boolean = true;
  timeout: any = null;

  constructor(private api: ApiService, private route: ActivatedRoute, private toastr: ToastrService, private translator: TranslateService) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid') as string;
  }

  onSearch(event: any) {
    clearTimeout(this.timeout);
    var $this = this;
    this.timeout = setTimeout(function () {
      if (event.keyCode != 13) {
        $this.executeSearch(event);
      }
    }, 500);
  }

  executeSearch(value: string) {
    if (value.trim()) {
      this.loading = true;
      this.showOverlay = true;
      this.searchResults = undefined;

      let params = new HttpParams()
        .set('search', value.trim());
      this.api.getSimpleData(`/guilds/${this.guildId}/dashboard/search`, true, params).subscribe(data => {
        this.searchResults = data;
        this.loading = false;
      }, error => {
        console.error(error);
        this.toastr.error(this.translator.instant('Quicksearch.SearchFailed'));
        this.loading = false;
      });
    } else {
      this.loading = false;
      this.showOverlay = false;
    }
  }
}
