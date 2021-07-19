import { Input } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { Guild } from 'src/app/models/Guild';

@Component({
  selector: 'app-guild-icon',
  templateUrl: './guild-icon.component.html',
  styleUrls: ['./guild-icon.component.css']
})
export class GuildIconComponent implements OnInit {

  @Input() public guild?: Guild = undefined;
  @Input() public width: string = '24px';
  @Input() public height: string = '24px';

  constructor() { }

  ngOnInit(): void {
    console.log("test", this.width)
  }

}
