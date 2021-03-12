import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-guild-dashboard',
  templateUrl: './guild-dashboard.component.html',
  styleUrls: ['./guild-dashboard.component.css']
})
export class GuildDashboardComponent implements OnInit {

  constructor(private toastr: ToastrService) { }

  ngOnInit(): void {
    this.toastr.success("hi");
  }

}
