import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-adminlist',
  templateUrl: './adminlist.component.html',
  styleUrls: ['./adminlist.component.css']
})
export class AdminlistComponent implements OnInit {

  @Input() public titleKey: string = "";
  @Input() public showList: string[] | undefined = []
  @Input() public loading: boolean = true;
  @Input() public emote: string = "person";
  public showAll: boolean = false;
  constructor() { }

  ngOnInit(): void {
  }

}
