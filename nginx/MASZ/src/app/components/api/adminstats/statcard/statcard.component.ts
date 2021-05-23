import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-statcard',
  templateUrl: './statcard.component.html',
  styleUrls: ['./statcard.component.css']
})
export class StatcardComponent implements OnInit {

  @Input() title?: string;
  @Input() text?: string | number | undefined;
  @Input() emote?: string ;
  constructor() { }

  ngOnInit(): void {
  }

}
