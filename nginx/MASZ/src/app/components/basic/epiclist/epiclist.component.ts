import { Component, Input, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { InfoPanel } from 'src/app/models/InfoPanel';

@Component({
  selector: 'app-epiclist',
  templateUrl: './epiclist.component.html',
  styleUrls: ['./epiclist.component.css']
})
export class EpiclistComponent implements OnInit {

  @Input() loading: boolean = true;
  @Input() items: InfoPanel[] = [];

  constructor() { }

  ngOnInit(): void {
  }
}
