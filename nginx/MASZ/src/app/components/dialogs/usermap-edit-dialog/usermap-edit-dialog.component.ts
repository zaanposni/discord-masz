import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UserMappingDto } from 'src/app/models/UserMappingDto';

@Component({
  selector: 'app-usermap-edit-dialog',
  templateUrl: './usermap-edit-dialog.component.html',
  styleUrls: ['./usermap-edit-dialog.component.css']
})
export class UsermapEditDialogComponent implements OnInit {

  public initRows = 1;
  constructor(@Inject(MAT_DIALOG_DATA) public settings: UserMappingDto) { }

  ngOnInit(): void {
    this.initRows = Math.min(this.settings.reason.split(/\r\n|\r|\n/).length, 15);
  }

}
