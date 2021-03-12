import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-quicksearch-moderation-result',
  templateUrl: './quicksearch-moderation-result.component.html',
  styleUrls: ['./quicksearch-moderation-result.component.css']
})
export class QuicksearchModerationResultComponent implements OnInit {

  @Input() entry!: any;
  constructor() { }

  ngOnInit(): void {
  }

}
