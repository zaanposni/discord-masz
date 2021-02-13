import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { InfoPanel } from '../models/InfoPanel';
import { ApiService } from '../services/api.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.scss']
})
export class IndexComponent implements OnInit {

  infoPanels!: Observable<InfoPanel[]>;

  constructor(private authService: AuthService, private api: ApiService) { }

  ngOnInit(): void {
    this.infoPanels = this.api.getSimpleData('/static/indexpage.json?v=1.8.2', false);
  }

}
