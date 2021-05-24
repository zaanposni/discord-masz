import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { interval } from 'rxjs';
import { Adminstats } from 'src/app/models/Adminstats';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-adminstats',
  templateUrl: './adminstats.component.html',
  styleUrls: ['./adminstats.component.css']
})
export class AdminstatsComponent implements OnInit {

  private subscription?: any;
  private timeDifference?: number;
  public secondsToNewCache?: string = '--';
  public minutesToNewCache?: string = '--';
  public stats: ContentLoading<Adminstats> = { loading: true, content: undefined };

  constructor(private api: ApiService, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.reload();
  }

  public reload() {
    this.stats = { loading: true, content: undefined };
    this.subscription?.unsubscribe();
    this.api.getSimpleData(`/meta/adminstats`).subscribe((data: Adminstats) => {
      this.stats = { loading: false, content: data };
      this.subscription = interval(1000)
           .subscribe(x => { this.getTimeDifference(); });
    }, () => {
      this.stats.loading = false;
      this.toastr.error("Failed to load adminstats.");
    });
  }

  private getTimeDifference() {
    if (this.stats.content == undefined) return;
    if (new Date(this.stats.content!.nextCache ?? '') < new Date()) {
      this.reload();
      return;
    }
    this.timeDifference = new Date(this.stats.content!.nextCache ?? '').getTime() - new Date().getTime();
    let seconds = Math.floor((this.timeDifference) / 1000 % 60);
    this.secondsToNewCache = seconds < 10 ? `0${seconds}` : seconds.toString();
    let minutes = Math.floor((this.timeDifference) / (1000 * 60) % 60);
    this.minutesToNewCache = minutes < 10 ? `0${minutes}` : minutes.toString();
  }

  ngOnDestroy() {
    this.subscription?.unsubscribe();
 }
}
