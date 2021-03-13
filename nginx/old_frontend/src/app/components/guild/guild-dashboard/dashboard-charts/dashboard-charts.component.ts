import { HttpParams } from '@angular/common/http';
import { Component, EventEmitter, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ChartDataSets } from 'chart.js';
import { Label } from 'ng2-charts';
import { BehaviorSubject, Observable, of, Subject } from 'rxjs';
import { DashboardCharts } from 'src/app/models/DashboardCharts';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-dashboard-charts',
  templateUrl: './dashboard-charts.component.html',
  styleUrls: ['./dashboard-charts.component.scss']
})
export class DashboardChartsComponent implements OnInit {

  private guildId: string;
  public loading: boolean = true;
  public since: Date = new Date();
  private today: Date = new Date();

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
    this.guildId = this.route.snapshot.paramMap.get('guildid');
    this.since.setDate(this.since.getDate() - 91); // - 1/4year
    this.reload();
  }

  private convertTime(time: Date): string {
    return new Date(time).toDateString();
  }

  public reload() {
    this.loading = true;
    
    let params = new HttpParams()
      .set('since', Math.floor(new Date(this.since).getTime() / 1000).toString());
    
    this.api.getSimpleData(`/guilds/${this.guildId}/dashboard/chart`, true, params).toPromise().then((data: DashboardCharts) => {
      this.caseChartData = [{ data: [ 0, ...data.modCases.map(x => x.count), 0 ], label: 'Count' }];
      this.caseChartLabels = [ this.since.toString(), ...data.modCases.map(x => this.convertTime(x.time)), this.convertTime(this.today) ];

      this.punishmentChartData = [{ data: [ 0, ...data.punishments.map(x => x.count), 0 ], label: 'Count' }];
      this.punishmentChartLabels = [ this.since.toString(), ...data.punishments.map(x => this.convertTime(x.time)), this.convertTime(this.today) ];

      this.moderationChartData = [{ data: [ 0, ...data.autoModerations.map(x => x.count), 0 ], label: 'Count' }];
      this.moderationChartLabels = [ this.since.toString(), ...data.autoModerations.map(x => this.convertTime(x.time)), this.convertTime(this.today) ];

      this.maxSubject$.next(Math.max( ...data.modCases.map(x => x.count), ...data.punishments.map(x => x.count), ...data.autoModerations.map(x => x.count) ));

      this.loading = false;
    }, () => { this.loading = false; });
  }

  public resetToDefault() {
    this.since = new Date();
    this.since.setDate(this.since.getDate() - 91); // - 1/4year
    this.reload();
  }
}
