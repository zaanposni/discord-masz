import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ChartDataSets, ChartOptions, ChartType } from 'chart.js';
import { BaseChartDirective, Color, Label } from 'ng2-charts';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-count-chart',
  templateUrl: './count-chart.component.html',
  styleUrls: ['./count-chart.component.css']
})
export class CountChartComponent implements OnInit {

  @ViewChild(BaseChartDirective) chart: BaseChartDirective | undefined;
  @Input() titleKey: string = '';
  @Input() max!: Observable<number>;
  @Input() chartLoading: boolean = true;
  @Input() public chartData: ChartDataSets[] = [];
  @Input() public chartLabels: Label[] = [];
  public chartOptions: (ChartOptions & { annotation: any } | any) = {
    title: {
      text: '',
      display: true,
      fontColor: 'rgba(232, 230, 227, 1)'
    },
    responsive: false,
    maintainAspectRatio: false,
    legend: {
      display: false
    },
    scales: {
      xAxes: [{
        type: 'time',
        time: {
          unit: 'month'
        },
        gridLines: {
          color: "rgba(0, 0, 0, 0)",
        }
      }],
      yAxes: [{
        ticks: {
          beginAtZero: true,
          userCallback: function(label: any, index: any, labels: any) {
            // when the floored value is the same as the value we have a whole number
            if (Math.floor(label) === label) {
                return label;
            }

        },
        },
        gridLines: {
          color: "rgba(0, 0, 0, 0)",
        } 
      }]
    }
  };
  public chartColors: Color[] = [
    {
      borderColor: 'rgb(172, 0, 15)',
      backgroundColor: '#e91e63'
    },
  ];
  public chartLegend = true;
  public chartType: ChartType = 'bar';
  public chartPlugins: any = [];

  constructor(private translator: TranslateService) { }

  ngOnInit(): void {
    this.chartOptions['title']['text'] = this.translator.instant(this.titleKey);
    this.translator.onLangChange.subscribe(() => {
      this.chartOptions['title']['text'] = this.translator.instant(this.titleKey);
      this.chart?.ngOnInit();
    });

    this.max?.subscribe((data) => {
      this.chartOptions['scales']['yAxes'][0]['ticks']['max'] = data;
    });
  }
}
