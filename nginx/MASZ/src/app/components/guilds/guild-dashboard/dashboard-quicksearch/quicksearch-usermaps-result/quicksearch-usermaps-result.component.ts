import { Component, Input, OnInit } from '@angular/core';
import { UserMappingView } from 'src/app/models/UserMappingView';

@Component({
  selector: 'app-quicksearch-usermaps-result',
  templateUrl: './quicksearch-usermaps-result.component.html',
  styleUrls: ['./quicksearch-usermaps-result.component.css']
})
export class QuicksearchUsermapsResultComponent implements OnInit {

  @Input() usermaps!: UserMappingView[];
  @Input() searchedFor!: string;
  constructor() { }

  ngOnInit(): void {
  }

}
