import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ChartDataSets, ChartOptions, ChartType } from 'chart.js';
import { BaseChartDirective, Color, Label } from 'ng2-charts';
import { ToastrService } from 'ngx-toastr';
import { APIEnumTypes } from 'src/app/models/APIEmumTypes';
import { AutoModerationType } from 'src/app/models/AutoModerationType';
import { AutomodSplit } from 'src/app/models/AutomodSplit';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { ApiService } from 'src/app/services/api.service';
import { EnumManagerService } from 'src/app/services/enum-manager.service';

@Component({
  selector: 'app-dashboard-automod-split',
  templateUrl: './dashboard-automod-split.component.html',
  styleUrls: ['./dashboard-automod-split.component.css']
})
export class DashboardAutomodSplitComponent implements OnInit {

  @ViewChild(BaseChartDirective) chart: BaseChartDirective | undefined;
  public loading: boolean = true;
  public foundContent: boolean = false;
  public chartData: ChartDataSets[] = [];
  public chartLabels: Label[] = [];
  public chartOptions: (ChartOptions & { annotation: any } | any) = {
    title: {
      text: '',
      display: true,
      fontColor: 'rgba(232, 230, 227, 1)'
    },
    responsive: true,
    maintainAspectRatio: true,
    legend: {
      display: true,
      labels: {
        fontColor: 'white'
      }
    }
  };
  public chartColors: Color[] = [
    {
      borderColor: 'rgba(18, 18, 18, 0.2)',
      backgroundColor: ['#d84315', '#f9a825', '#2e7d32', '#00695c', '#0277bd', '#1e0ead', '#6a1b9a', '#cc1097']
    },
  ];
  public chartLegend = true;
  public chartType: ChartType = 'pie';
  public chartPlugins: any = [];
  public splittedData: AutomodSplit[] = [];

  constructor(private route: ActivatedRoute, private api: ApiService, private toastr: ToastrService, private translator: TranslateService, private enumManager: EnumManagerService) { }

  ngOnInit(): void {
    const guildId = this.route.snapshot.paramMap.get('guildid');
    this.initialize(guildId as string);
  }

  initialize(guildId: string) {
    this.loading = true;
    this.foundContent = false;
    this.api.getSimpleData(`/guilds/${guildId}/dashboard/automodchart`).subscribe((data: AutomodSplit[]) => {
      this.splittedData = data;
      this.chartData = [{ data: data.map(x => x.count), label: 'Count' }];
      if (data.length) {
        this.foundContent = true;
        this.enumManager.getEnum(APIEnumTypes.AUTOMODTYPE).subscribe(data => {
          this.chartLabels = this.splittedData.map(d => data.find(x => x.key === d.type)?.value) as Label[];
          this.chart?.ngOnInit();
        });
      }
      this.loading = false;
    }, error => {
      console.error(error);
      this.loading = false;
      this.toastr.error(this.translator.instant('AutomodSplit.FailedToLoad'));
    });
  }
}
