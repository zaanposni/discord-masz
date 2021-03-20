import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { InfoPanel } from 'src/app/models/InfoPanel';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-donate',
  templateUrl: './donate.component.html',
  styleUrls: ['./donate.component.css']
})
export class DonateComponent implements OnInit {

  public loading: boolean = true;
  public items: InfoPanel[] = [];
  constructor(private api: ApiService) { }

  ngOnInit(): void {
    this.reload();
  }

  private reload() {
    this.loading = true;
    this.items = [];
    this.api.getSimpleData('/static/donate.json', false).subscribe((data) => {
      this.items = data;
      this.loading = false;
    }, () => {
      this.loading = false;
    });
  }
}
