import { HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDatepicker } from '@angular/material/datepicker';
import { ActivatedRoute } from '@angular/router';
import { ChartDataSets } from 'chart.js';
import { Label } from 'ng2-charts';
import { Observable, Subject } from 'rxjs';
import { DashboardCharts } from 'src/app/models/DashboardCharts';
import { DbCount } from 'src/app/models/DbCount';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-dashboard-charts',
  templateUrl: './dashboard-charts.component.html',
  styleUrls: ['./dashboard-charts.component.css']
})
export class DashboardChartsComponent implements OnInit {

  private guildId!: string;
  public loading: boolean = true;
  public since: Date = new Date();

  public caseChartData: ChartDataSets[] = [];
  public caseChartLabels: Label[] = [];
  public punishmentChartData: ChartDataSets[] = [];
  public punishmentChartLabels: Label[] = [];
  public moderationChartData: ChartDataSets[] = [];
  public moderationChartLabels: Label[] = [];
  private maxSubject$ = new Subject<number>();
  public max: Observable<number> = this.maxSubject$.asObservable();

  constructor(private api: ApiService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid') as string;
    this.since.setDate(this.since.getDate() - 365); // - 1year
    this.reload();
  }

  private convertTime(e: DbCount|Date): string {
    if (e instanceof Date) {
      return new Date(e.getFullYear(), e.getMonth(), 1, 0, 0, 0, 0).toDateString();
    } else {
      return new Date(e.year, e.month-1, 1, 0, 0, 0, 0).toDateString();
    }
  }

  public reload() {
    this.loading = true;

    let params = new HttpParams()
      .set('since', Math.floor(new Date(this.since).getTime() / 1000).toString());

    this.api.getSimpleData(`/guilds/${this.guildId}/dashboard/chart`, true, params).subscribe((data: DashboardCharts) => {

      const sinceInsert = this.convertTime(this.since);

      if (data.modCases.map(x => this.convertTime(x)).includes(sinceInsert)) {
        this.caseChartData = [{ data: data.modCases.map(x => x.count), label: 'Count' }];
        this.caseChartLabels = data.modCases.map(x => this.convertTime(x));
      } else {
        this.caseChartData = [{ data: [ 0, ...data.modCases.map(x => x.count) ], label: 'Count' }];
        this.caseChartLabels = [ sinceInsert.toString(), ...data.modCases.map(x => this.convertTime(x)) ];
      }

      if (data.punishments.map(x => this.convertTime(x)).includes(sinceInsert)) {
        this.punishmentChartData = [{ data: data.punishments.map(x => x.count), label: 'Count' }];
        this.punishmentChartLabels = data.punishments.map(x => this.convertTime(x));
      } else {
        this.punishmentChartData = [{ data: [ 0, ...data.punishments.map(x => x.count) ], label: 'Count' }];
        this.punishmentChartLabels = [ sinceInsert.toString(), ...data.punishments.map(x => this.convertTime(x)) ];
      }

      if (data.autoModerations.map(x => this.convertTime(x)).includes(sinceInsert)) {
        this.moderationChartData = [{ data: data.autoModerations.map(x => x.count), label: 'Count' }];
        this.moderationChartLabels = data.autoModerations.map(x => this.convertTime(x));
      } else {
        this.moderationChartData = [{ data: [ 0, ...data.autoModerations.map(x => x.count) ], label: 'Count' }];
        this.moderationChartLabels = [ sinceInsert.toString(), ...data.autoModerations.map(x => this.convertTime(x)) ];
      }

      this.maxSubject$.next(Math.max( ...data.modCases.map(x => x.count), ...data.punishments.map(x => x.count), 10 ));

      this.loading = false;
    }, () => { this.loading = false; });
  }

  public resetToDefault() {
    this.since = new Date();
    this.since.setDate(this.since.getDate() - 365); // - 1year
    this.reload();
  }

}
