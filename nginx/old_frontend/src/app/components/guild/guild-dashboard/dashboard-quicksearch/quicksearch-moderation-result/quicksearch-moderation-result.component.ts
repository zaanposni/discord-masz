import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AutoModerationEventTableEntry } from 'src/app/models/AutoModerationEventTableEntry';

@Component({
  selector: 'app-quicksearch-moderation-result',
  templateUrl: './quicksearch-moderation-result.component.html',
  styleUrls: ['./quicksearch-moderation-result.component.scss']
})
export class QuicksearchModerationResultComponent implements OnInit {

  @Input() entry!: any;

  constructor(public router: Router) { }

  ngOnInit(): void {
  }

}
