import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { InfoPanel } from 'src/app/models/InfoPanel';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-donate',
  templateUrl: './donate.component.html',
  styleUrls: ['./donate.component.scss']
})
export class DonateComponent implements OnInit {

  panels!: Observable<InfoPanel[]>;

  constructor(private api: ApiService) { }

  ngOnInit(): void {
    this.panels = this.api.getSimpleData('/static/donate.json?v=1.10', false);
  }

}
