import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Adminstats } from 'src/app/models/Adminstats';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-adminstats',
  templateUrl: './adminstats.component.html',
  styleUrls: ['./adminstats.component.css']
})
export class AdminstatsComponent implements OnInit {

  public stats: ContentLoading<Adminstats> = { loading: true, content: undefined };

  constructor(private api: ApiService, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.reload();
  }

  public reload() {
    this.stats = { loading: true, content: undefined };
    this.api.getSimpleData(`/meta/adminstats`).subscribe((data: Adminstats) => {
      this.stats = { loading: false, content: data };
    }, () => {
      this.stats.loading = false;
      this.toastr.error("Failed to load adminstats.");
    });
  }
}
