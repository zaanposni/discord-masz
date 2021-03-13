import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ChartDataSets, ChartOptions, ChartType } from 'chart.js';
import { Color, Label } from 'ng2-charts';
import { ToastrService } from 'ngx-toastr';
import { AutoModerationType } from 'src/app/models/AutoModerationType';
import { AutomodSplit } from 'src/app/models/AutomodSplit';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-dashboard-automod-split',
  templateUrl: './dashboard-automod-split.component.html',
  styleUrls: ['./dashboard-automod-split.component.css']
})
export class DashboardAutomodSplitComponent implements OnInit {

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
    responsive: false,
    maintainAspectRatio: false,
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
      backgroundColor: ['#d84315', '#f9a825', '#2e7d32', '#00695c', '#0277bd', '#6a1b9a']
    },
  ];
  public chartLegend = true;
  public chartType: ChartType = 'pie';
  public chartPlugins: any = [];
  
  constructor(private route: ActivatedRoute, private api: ApiService, private toastr: ToastrService) { }

  ngOnInit(): void {
    const guildId = this.route.snapshot.paramMap.get('guildid');
    this.initialize(guildId as string);
  }

  initialize(guildId: string) {
    this.loading = true;
    this.foundContent = false;
    this.api.getSimpleData(`/guilds/${guildId}/dashboard/automodchart`).subscribe((data: AutomodSplit[]) => {
      this.chartData = [{ data: data.map(x => x.count), label: 'Count' }];
      this.chartLabels = data.map(x => AutoModerationType[x.type]);

      if (data.length) {
        this.foundContent = true;
      }
      this.loading = false;
    }, () => {
      this.loading = false;
      this.toastr.error("Failed to load guild autmod split chart.");
    });
  }

}
